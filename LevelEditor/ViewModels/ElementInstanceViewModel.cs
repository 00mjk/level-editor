using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Bitcraft.UI.Core;
using LayerDataReaderWriter;
using LayerDataReaderWriter.V9;
using LevelEditor.Behaviors;
using LevelEditor.Core;
using LevelEditor.Core.Settings.V1;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.ViewModels
{
    public class ElementInstanceViewModel : RootedViewModel, IDropTarget, IDisposable, IReadOnlyElementInstanceViewModel
    {
        public ushort UniqueIdentifier { get; private set; }

        private double unitPositionX;
        public double UnitPositionX
        {
            get { return unitPositionX; }
            set
            {
                var newValue = Math.Max(0.0, Math.Min(value, 1.0));
                SetDataValue(ref unitPositionX, newValue);
            }
        }

        private double unitPositionY;
        public double UnitPositionY
        {
            get { return unitPositionY; }
            set
            {
                var newValue = Math.Max(0.0, Math.Min(value, 1.0));
                SetDataValue(ref unitPositionY, newValue);
            }
        }

        private double pivotX = 0.5;
        public double PivotX
        {
            get { return pivotX; }
            set { SetDataValue(ref pivotX, value); }
        }

        private double pivotY = 0.5;
        public double PivotY
        {
            get { return pivotY; }
            set { SetDataValue(ref pivotY, value); }
        }

        private double angle = 0.0;
        public double Angle
        {
            get { return angle; }
            set { SetDataValue(ref angle, value); }
        }

        private double scaleX = 1.0;
        public double ScaleX
        {
            get { return scaleX; }
            set { SetDataValue(ref scaleX, value); }
        }

        private double dpiBasedScaleX = 1.0;
        public double DpiBasedScaleX
        {
            get { return dpiBasedScaleX; }
            private set { SetValue(ref dpiBasedScaleX, value); }
        }

        private double scaleY = 1.0;
        public double ScaleY
        {
            get { return scaleY; }
            set { SetDataValue(ref scaleY, value); }
        }

        private double dpiBasedScaleY = 1.0;
        public double DpiBasedScaleY
        {
            get { return dpiBasedScaleY; }
            private set { SetValue(ref dpiBasedScaleY, value); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (SetValue(ref isSelected, value))
                    parent.EvaluateSelected();
            }
        }

        private LayerBlockViewModel parent;

        private byte layer;
        public byte Layer
        {
            get { return layer; }
            set { SetDataValue(ref layer, value); }
        }

        private byte depth;
        public byte Depth
        {
            get { return depth; }
            set { SetDataValue(ref depth, value); }
        }

        private ushort type;
        public ushort Type
        {
            get { return type; }
            set
            {
                if (SetDataValue(ref type, value))
                    UpdateDpiBasedScale();
            }
        }

        private ColliderType colliderType = ColliderType.Square;
        public ColliderType ColliderType
        {
            get { return colliderType; }
            set { SetDataValue(ref colliderType, value); }
        }

        private ObservableCollection<ComponentInstanceViewModel> components = new ObservableCollection<ComponentInstanceViewModel>();
        public IReadOnlyObservableCollection<ComponentInstanceViewModel> Components { get; private set; }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            parent.EvaluateSelected();
        }

        public ElementInstanceViewModel(ushort uid, RootViewModel root, LayerBlockViewModel parent, LayerBlockElementDefinition definition)
            : base(root)
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            Initialize(parent, uid, definition.Type);

            ScaleX = definition.DefaultScaleX;
            ScaleY = definition.DefaultScaleY;
            PivotX = definition.DefaultPivotX;
            PivotY = definition.DefaultPivotY;

            var settings = Root.Settings;
            if (settings != null && settings.GameBoard != null && settings.GameBoard.LayerSettings != null)
                Layer = settings.GameBoard.LayerSettings.Default;
        }

        public ElementInstanceViewModel(ushort uid, RootViewModel root, LayerBlockViewModel parent, LayerBlockElement element)
            : base(root)
        {
            Initialize(parent, uid, element.Type);

            UnitPositionX = Utility.DataFloatRound(element.Tx);
            UnitPositionY = 1.0 - Utility.DataFloatRound(element.Ty);
            PivotX = Utility.DataFloatRound(element.Px);
            PivotY = Utility.DataFloatRound(element.Py);
            Angle = Utility.DataFloatRound(element.Angle);
            ScaleX = Utility.DataFloatRound(element.Sx);
            ScaleY = Utility.DataFloatRound(element.Sy);
            ColliderType = element.ColliderType;
            Layer = element.Layer;
            Depth = element.Depth;

            if (element.Components != null)
            {
                foreach (var component in element.Components)
                {
                    var compo = Root.ComponentToolBox.GetComponentFromType(component.Type);
                    if (compo != null)
                        components.Add(new ComponentInstanceViewModel(Root, this, compo, component.Properties));
                }
            }
        }

        private void Initialize(LayerBlockViewModel parent, ushort uid, ushort elementType)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            this.parent = parent;
            UniqueIdentifier = uid;
            Type = elementType;

            UpdateDpiBasedScale();

            Components = new Bitcraft.UI.Core.ReadOnlyObservableCollection<ComponentInstanceViewModel>(components);

            components.CollectionChanged += components_CollectionChanged;
        }

        private void components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Root.IsDataModified = true;
            OnPropertyChanged("Components");
        }

        public void RemoveComponent(ComponentInstanceViewModel component)
        {
            components.Remove(component);
        }

        private void UpdateDpiBasedScale()
        {
            double localDpiX;
            double localDpiY;
            double localOutputDpiX;
            double localOutputDpiY;

            ImageManager.GetImageDpi(Type, out localDpiX, out localDpiY, out localOutputDpiX, out localOutputDpiY);

            DpiBasedScaleX = Math.Round(localDpiX / localOutputDpiX, 3);
            DpiBasedScaleY = Math.Round(localDpiY / localOutputDpiY, 3);
        }

        public LayerBlockElement ProduceLayerBlockElement()
        {
            return new LayerBlockElement(
                UniqueIdentifier,
                (float)UnitPositionX,
                (float)(1.0 - UnitPositionY),
                Layer,
                Depth,
                (float)PivotX,
                (float)PivotY,
                (float)Angle,
                (float)ScaleX,
                (float)ScaleY,
                Type,
                ColliderType,
                components
                    .Select(c => c.ProduceComponent())
                    .Where(c => c != null)
                    .ToArray()
                );
        }

        public DragDropEffects Drag(DropTargetEventType type, IDataObject data, Point position)
        {
            if (data.GetDataPresent(typeof(ComponentViewModel)) == false)
                return DragDropEffects.None;

            return DragDropEffects.Copy;
        }

        public void Drop(IDataObject data, Point position)
        {
            var source = (ComponentViewModel)data.GetData(typeof(ComponentViewModel));
            components.Add(new ComponentInstanceViewModel(Root, this, source));
        }

        public void Dispose()
        {
            foreach (var component in components)
                component.Dispose();
            components.CollectionChanged -= components_CollectionChanged;
        }

        IReadOnlyObservableCollection<IReadOnlyComponentInstanceViewModel> IReadOnlyElementInstanceViewModel.Components
        {
            get { return Components; }
        }
    }
}
