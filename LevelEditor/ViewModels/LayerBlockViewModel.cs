using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Bitcraft.Core;
using Bitcraft.UI.Core;
using LayerDataReaderWriter;
using LayerDataReaderWriter.V9;
using LevelEditor.Behaviors;
using LevelEditor.Core;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using LevelEditor.Features.Selection;
using Bitcraft.Core.Extensions;
using Bitcraft.UI.Core.Extensions;

namespace LevelEditor.ViewModels
{
    public class GroupedDoubleValueViewModel : RootedViewModel, IReadOnlyGroupedDoubleValueViewModel
    {
        private double value;
        public double Value
        {
            get { return value; }
            set
            {
                if (SetValue(ref this.value, value))
                {
                    HasExtendedValues = false;
                    if (setter != null)
                        setter(value);
                }
            }
        }

        private bool hasExtendedValues;
        public bool HasExtendedValues
        {
            get { return hasExtendedValues; }
            private set { SetValue(ref hasExtendedValues, value); }
        }

        public double Min { get; private set; }
        public double Max { get; private set; }
        public double Avg { get; private set; }

        private Action<double> setter;

        public GroupedDoubleValueViewModel(RootViewModel root, Action<double> setter, double value)
            : base(root)
        {
            this.setter = setter;

            Value = value;
            HasExtendedValues = false;
        }

        public GroupedDoubleValueViewModel(RootViewModel root, Action<double> setter, double min, double max, double avg)
            : base(root)
        {
            this.setter = setter;

            Value = 0.0;
            HasExtendedValues = true;
            Min = min;
            Max = max;
            Avg = avg;
        }
    }

    public class SelectedBlockElementsViewModel : RootedViewModel, IReadOnlySelectedBlockElementsViewModel
    {
        private ElementInstanceViewModel[] selected;

        public SelectedBlockElementsViewModel(RootViewModel root, ElementInstanceViewModel[] selected)
            : base(root)
        {
            if (selected == null)
                throw new ArgumentNullException("selected");

            this.selected = selected;

            UniqueIdentifier = GetUngroupableValue(e => e.UniqueIdentifier);

            UnitPositionX = GetGroupedValue(e => e.UnitPositionX, v => InternalSetValue(elem => elem.UnitPositionX = v));
            UnitPositionY = GetGroupedValue(e => e.UnitPositionY, v => InternalSetValue(elem => elem.UnitPositionY = v));
            PivotX = GetGroupedValue(e => e.PivotX, v => InternalSetValue(elem => elem.PivotX = v));
            PivotY = GetGroupedValue(e => e.PivotY, v => InternalSetValue(elem => elem.PivotY = v));
            Angle = GetGroupedValue(e => e.Angle, v => InternalSetValue(elem => elem.Angle = v));
            ScaleX = GetGroupedValue(e => e.ScaleX, v => InternalSetValue(elem => elem.ScaleX = v));
            ScaleY = GetGroupedValue(e => e.ScaleY, v => InternalSetValue(elem => elem.ScaleY = v));

            layer = GetUngroupableValue(e => e.Layer);
            depth = GetUngroupableValue(e => e.Depth);
            type = GetUngroupableValue(e => e.Type);
            colliderType = GetUngroupableValue(e => e.ColliderType);
        }

        public ushort UniqueIdentifier { get; private set; }

        public GroupedDoubleValueViewModel UnitPositionX { get; private set; }
        public GroupedDoubleValueViewModel UnitPositionY { get; private set; }
        public GroupedDoubleValueViewModel PivotX { get; private set; }
        public GroupedDoubleValueViewModel PivotY { get; private set; }
        public GroupedDoubleValueViewModel Angle { get; private set; }
        public GroupedDoubleValueViewModel ScaleX { get; private set; }
        public GroupedDoubleValueViewModel ScaleY { get; private set; }

        private byte layer;
        public byte Layer
        {
            get { return layer; }
            set { InternalSetValue(x => x.Layer = value); }
        }

        private byte depth;
        public byte Depth
        {
            get { return depth; }
            set
            {
                depth = value; // <-- what the fuck ??????????????? 
                // WPF pulls data from the previous DataContext, so the old one must be forcibly set
                // and it seems this happens only for Depth, not for other properties

                InternalSetValue(x => x.Depth = value);
            }
        }

        private ushort type;
        public ushort Type
        {
            get { return type; }
            set { InternalSetValue(x => x.Type = value); }
        }

        private ColliderType colliderType;
        public ColliderType ColliderType
        {
            get { return colliderType; }
            set { InternalSetValue(x => x.ColliderType = value); }
        }

        private static Dictionary<string, MethodInfo> setters = new Dictionary<string, MethodInfo>();

        internal void InternalSetValue(Action<ElementInstanceViewModel> setter)
        {
            foreach (var s in selected)
                setter(s);
        }

        private GroupedDoubleValueViewModel GetGroupedValue(Func<ElementInstanceViewModel, double> get, Action<double> set)
        {
            if (selected.Any() == false)
                return null;

            var first = get(selected.First());

            if (selected.All(x => get(x) == first))
            {
                return new GroupedDoubleValueViewModel(Root, set, first);
            }
            else
            {
                var min = selected.Min(x => get(x));
                var max = selected.Max(x => get(x));
                var avg = selected.Average(x => get(x));

                return new GroupedDoubleValueViewModel(Root, set, min, max, avg);
            }
        }

        private T GetUngroupableValue<T>(Func<ElementInstanceViewModel, T> get)
        {
            if (selected.Any() == false)
                return default(T);

            var first = get(selected.First());
            if (first == null)
                return default(T);

            if (selected.All(x => first.Equals(get(x))))
                return first;

            return default(T);
        }

        IReadOnlyGroupedDoubleValueViewModel IReadOnlySelectedBlockElementsViewModel.UnitPositionX
        {
            get { return UnitPositionX; }
        }

        IReadOnlyGroupedDoubleValueViewModel IReadOnlySelectedBlockElementsViewModel.UnitPositionY
        {
            get { return UnitPositionY; }
        }

        IReadOnlyGroupedDoubleValueViewModel IReadOnlySelectedBlockElementsViewModel.PivotX
        {
            get { return PivotX; }
        }

        IReadOnlyGroupedDoubleValueViewModel IReadOnlySelectedBlockElementsViewModel.PivotY
        {
            get { return PivotY; }
        }

        IReadOnlyGroupedDoubleValueViewModel IReadOnlySelectedBlockElementsViewModel.Angle
        {
            get { return Angle; }
        }

        IReadOnlyGroupedDoubleValueViewModel IReadOnlySelectedBlockElementsViewModel.ScaleX
        {
            get { return ScaleX; }
        }

        IReadOnlyGroupedDoubleValueViewModel IReadOnlySelectedBlockElementsViewModel.ScaleY
        {
            get { return ScaleY; }
        }
    }

    public class LayerBlockViewModel : RootedViewModel, IDropTarget, IDisposable, IReadOnlyLayerBlockViewModel
    {
        private readonly UniqueIdentifierManager uniqueIdentifierManager = new UniqueIdentifierManager();

        private byte difficulty;
        public byte Difficulty
        {
            get { return difficulty; }
            set { SetDataValue(ref difficulty, value); }
        }

        public byte Identifier { get; private set; }

        public ICommand MoveUpCommand { get; private set; }
        public ICommand MoveDownCommand { get; private set; }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetDataValue(ref isEnabled, value); }
        }

        public LayerBlockInfoViewModel Info { get; private set; }

        private SelectedBlockElementsViewModel selectedValues;
        public SelectedBlockElementsViewModel SelectedValues
        {
            get { return selectedValues; }
            private set { SetValue(ref selectedValues, value); }
        }

        private ObservableCollection<ElementInstanceViewModel> instances = new ObservableCollection<ElementInstanceViewModel>();
        public IReadOnlyObservableCollection<ElementInstanceViewModel> Instances { get; private set; }

        private int instanceCount;
        public int InstanceCount
        {
            get { return instanceCount; }
            private set { SetValue(ref instanceCount, value); }
        }

        public LayerDataViewModel Parent { get; private set; }

        private bool isDragging;
        public bool IsDragging
        {
            get { return isDragging; }
            set { SetValue(ref isDragging, value); }
        }

        public ICommand DeleteBlockCommand { get; private set; }
        public ICommand SuppressCommand { get; private set; }

        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }

        public ICommand SelectAllCommand { get; private set; }
        public ICommand DeselectAllCommand { get; private set; }
        public ICommand InvertSelectionCommand { get; private set; }

        public LayerBlockViewModel(RootViewModel root, LayerDataViewModel parent)
            : base(root)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            Parent = parent;
            Info = new LayerBlockInfoViewModel(
                this,
                Root.Settings.GameBoard.GameBoardWidth,
                Root.Settings.GameBoard.GameBoardHeight);

            Initialize();

            Identifier = Utility.FindAvailableUniqueIdentifier(Root.LayerData.Blocks.Select(b => b.Identifier));
        }

        public LayerBlockViewModel(RootViewModel root, LayerDataViewModel parent, LayerBlock layerBlock)
            : base(root)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (layerBlock == null)
                throw new ArgumentNullException("layerBlock");

            Parent = parent;
            Info = new LayerBlockInfoViewModel(
                this,
                Utility.DataFloatRound(layerBlock.Width),
                Utility.DataFloatRound(layerBlock.Height),
                layerBlock.UserFlags);

            Initialize();

            Identifier = layerBlock.Identifier;
            Difficulty = layerBlock.Difficulty;

            IsEnabled = layerBlock.SystemFlags.IsEnabled;

            LoadBlockElements(layerBlock);
        }

        private void Initialize()
        {
            Instances = new Bitcraft.UI.Core.ReadOnlyObservableCollection<ElementInstanceViewModel>(instances);

            Info.SnapX = Root.Settings.GameBoard.GridSnapX;
            Info.SnapY = Root.Settings.GameBoard.GridSnapY;

            Info.Zoom = Root.Settings.GameBoard.Zoom;

            DeleteBlockCommand = new AnonymousCommand(OnDeleteBlock);

            SuppressCommand = new AnonymousCommand(OnSuppress);

            CutCommand = new AnonymousCommand(OnCut);
            CopyCommand = new AnonymousCommand(OnCopy);
            PasteCommand = new AnonymousCommand(OnPaste);

            SelectAllCommand = new AnonymousCommand(OnSelectAll);
            DeselectAllCommand = new AnonymousCommand(OnDeselectAll);
            InvertSelectionCommand = new AnonymousCommand(OnInvertSelection);

            MoveUpCommand = new AnonymousCommand(OnMoveUp);
            MoveDownCommand = new AnonymousCommand(OnMoveDown);

            instances.CollectionChanged += instances_CollectionChanged;
        }

        private void OnMoveUp()
        {
            Parent.MoveBlockUp(this);
        }

        private void OnMoveDown()
        {
            Parent.MoveBlockDown(this);
        }

        private void OnCut(object parameter)
        {
            var clipboardData = GetClipboardData();
            //Clipboard.SetText(clipboardData);
            Clipboard.SetDataObject(clipboardData, false);
            OnSuppress();
        }

        private void OnCopy(object parameter)
        {
            var clipboardData = GetClipboardData();
            //Clipboard.SetText(clipboardData);
            Clipboard.SetDataObject(clipboardData, false);
        }

        private void OnPaste(object parameter)
        {
            try
            {
                var clipboardData = Clipboard.GetText();
                var xml = XElement.Parse(clipboardData);

                var elements = xml.Elements("e")
                    .Select(n => new LayerBlockElement(
                        uniqueIdentifierManager.GetNext(),
                        (float)((double)n.Attribute("x")),
                        (float)((double)n.Attribute("y")),
                        (byte)((uint)n.Attribute("z")),
                        (byte)((uint)n.Attribute("d")),
                        (float)((double)n.Attribute("px")),
                        (float)((double)n.Attribute("py")),
                        (float)((double)n.Attribute("a")),
                        (float)((double)n.Attribute("sx")),
                        (float)((double)n.Attribute("sy")),
                        (ushort)((uint)n.Attribute("t")),
                        (ColliderType)((uint)n.Attribute("ct")),
                        n.Elements("cs").Elements("c").Select(GetComponent).ToArray()))
                    .ToArray();

                foreach (var instance in Instances)
                    instance.IsSelected = false;

                var newInstances = elements
                    .Select(element => new ElementInstanceViewModel(element.ID, Root, this, element))
                    .ToArray();

                foreach (var instance in newInstances)
                    AddInstance(instance);

                foreach (var instance in newInstances)
                {
                    instance.IsSelected = false;
                    instance.IsSelected = true;
                }
            }
            catch
            {
            }
        }

        private Component GetComponent(XElement c)
        {
            var type = (string)c.Attribute("t");

            return new Component(
                type,
                c.Elements("ps").Elements("p").Select(GetComponentProperty).ToArray()
            );
        }

        private ComponentProperty GetComponentProperty(XElement p)
        {
            var name = (string)p.Attribute("n");
            var propertyType = (int)p.Attribute("t");
            switch ((ComponentPropertyType)propertyType)
            {
                case ComponentPropertyType.Boolean: return new ComponentProperty(name, (bool)p.Attribute("v"));
                case ComponentPropertyType.Integer: return new ComponentProperty(name, (int)p.Attribute("v"));
                case ComponentPropertyType.Float: return new ComponentProperty(name, (float)p.Attribute("v"));
                case ComponentPropertyType.String: return new ComponentProperty(name, (string)p.Attribute("v"));
                case ComponentPropertyType.Guid: return new ComponentProperty(name, (Guid)p.Attribute("v"));
            }
            return null;
        }

        private string GetClipboardData()
        {
            var xml = new XElement("es", Instances
                .Where(n => n.IsSelected)
                .Select(n => new XElement("e",
                    new XAttribute("x", n.UnitPositionX),
                    new XAttribute("y", 1.0 - n.UnitPositionY),
                    new XAttribute("z", n.Layer),
                    new XAttribute("d", n.Depth),
                    new XAttribute("px", n.PivotX),
                    new XAttribute("py", n.PivotY),
                    new XAttribute("a", n.Angle),
                    new XAttribute("sx", n.ScaleX),
                    new XAttribute("sy", n.ScaleY),
                    new XAttribute("t", (uint)n.Type),
                    new XAttribute("ct", (uint)n.ColliderType),
                    new XElement("cs", n.Components
                        .Select(c => new XElement("c", new XAttribute("t", c.Type),
                            new XElement("ps", c.Properties
                                .Select(p => CreateComponentPropertyXML(p))
                            ))
                        )
                    ))
                ));

            return xml.ToString();
        }

        private XElement CreateComponentPropertyXML(ComponentInstancePropertyViewModel c)
        {
            XAttribute value;

            switch (c.Type)
            {
                case ComponentPropertyType.Boolean: value = new XAttribute("v", c.BooleanValue); break;
                case ComponentPropertyType.Integer: value = new XAttribute("v", c.IntegerValue); break;
                case ComponentPropertyType.Float: value = new XAttribute("v", c.FloatValue); break;
                case ComponentPropertyType.String: value = new XAttribute("v", c.StringValue); break;
                case ComponentPropertyType.Guid: value = new XAttribute("v", c.GuidValue); break;
                default:
                    throw new InvalidOperationException();
            }

            return new XElement("p", new XAttribute("n", c.Name), new XAttribute("t", (int)c.Type), value);
        }

        private XElement CreateComponentPropertiesXML(ComponentInstanceViewModel c)
        {
            if (c == null || c.Properties.Count == 0)
                return null;

            return new XElement("ps", c.Properties.Select(x => CreateComponentPropertyXML(x)));
        }

        private void OnDeleteBlock()
        {
            Dispose();
            Parent.RemoveBlock(this);
        }

        private void OnSuppress()
        {
            var selectedColdList = Instances
                .Where(x => x.IsSelected)
                .ToArray();

            foreach (var selected in selectedColdList)
            {
                selected.Dispose();
                RemoveInstance(selected);
            }
        }

        private void OnSelectAll()
        {
            foreach (var item in Instances)
                item.IsSelected = true;
        }

        private void OnDeselectAll()
        {
            foreach (var item in Instances)
                item.IsSelected = false;
        }

        private void OnInvertSelection()
        {
            foreach (var item in Instances)
                item.IsSelected = !item.IsSelected;
        }

        private void instances_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Root.IsDataModified = true;
            InstanceCount = instances.Count;
        }

        public LayerBlock ProduceLayerBlock()
        {
            // TODO not 8 any more, now it is 256
            var flagArrayQuery = from i in Enumerable.Range(0, 8)
                                 let fs = Info.Flags.FirstOrDefault(f => f.FlagNumber == i)
                                 select fs != null ? fs.IsChecked : false;

            return new LayerBlock(
                Identifier,
                Difficulty,
                (float)Info.BlockWidth,
                (float)Info.BlockHeight,
                flagArrayQuery.ToArray(),
                new SystemFlags(IsEnabled),
                Instances
                    .Select(x => x.ProduceLayerBlockElement())
                    .OrderBy(e => e.Layer * 256 + e.Depth)
                    .ToArray());
        }

        public DragDropEffects Drag(DropTargetEventType type, IDataObject data, Point position)
        {
            if (data.GetDataPresent(typeof(ElementToolBoxElementViewModel)) == false)
                return DragDropEffects.None;

            IsDragging = true;

            return DragDropEffects.Copy;
        }

        public void Drop(IDataObject data, Point position)
        {
            IsDragging = false;

            if (data.GetDataPresent(typeof(ElementToolBoxElementViewModel)) == false)
                return;

            var element = (ElementToolBoxElementViewModel)data.GetData(typeof(ElementToolBoxElementViewModel));

            using (new Snapper(this))
            {
                var newValueX = position.X / Info.BlockWidth;
                var newValueY = position.Y / Info.BlockHeight;

                var newElement = new ElementInstanceViewModel(uniqueIdentifierManager.GetNext(), Root, this, element.Definition)
                {
                    UnitPositionX = Parent.IsSnappingToGrid ? ComputeSnapValueX(newValueX) : newValueX,
                    UnitPositionY = Parent.IsSnappingToGrid ? ComputeSnapValueY(newValueY) : newValueY,
                };
                AddInstance(newElement);

                OnDeselectAll();

                newElement.IsSelected = true;

                EvaluateSelected();
            }
        }

        public void Clear()
        {
            instances.Clear();
            uniqueIdentifierManager.Reset();
        }

        private void LoadBlockElements(LayerBlock block)
        {
            Clear();

            foreach (var elem in block.Elements)
                AddInstance(new ElementInstanceViewModel(elem.ID, Root, this, elem));
        }

        private ElementInstanceViewModel selectedElement;
        public ElementInstanceViewModel SelectedElement
        {
            get { return selectedElement; }
            private set { SetValue(ref selectedElement, value); }
        }

        internal void EvaluateSelected()
        {
            var selected = Instances.Where(x => x.IsSelected).ToArray();

            SelectedElement = selected.Length == 1 ? selected[0] : null;

            SelectedValues = new SelectedBlockElementsViewModel(Root, selected);
        }

        public double ComputeSnapValueX(double value)
        {
            var snapX = Info.SnapX / Info.BlockWidth;

            var temp1 = value / snapX;
            var temp2 = Math.Floor(temp1);
            if (temp1 - temp2 > 0.5)
                temp2++;

            return temp2 * snapX;
        }

        public double ComputeSnapValueY(double value)
        {
            var snapY = Info.SnapY / Info.BlockHeight;

            var temp1 = value / snapY;
            var temp2 = Math.Floor(temp1);
            if (temp1 - temp2 > 0.5)
                temp2++;

            return temp2 * snapY;
        }

        public void Dispose()
        {
            foreach (var elem in Instances)
                elem.Dispose();
            instances.CollectionChanged -= instances_CollectionChanged;
        }

        IReadOnlyLayerBlockInfoViewModel IReadOnlyLayerBlockViewModel.Info
        {
            get { return Info; }
        }

        IReadOnlySelectedBlockElementsViewModel IReadOnlyLayerBlockViewModel.SelectedValues
        {
            get { return SelectedValues; }
        }

        IReadOnlyObservableCollection<IReadOnlyElementInstanceViewModel> IReadOnlyLayerBlockViewModel.Instances
        {
            get { return Instances; }
        }

        IReadOnlyElementInstanceViewModel IReadOnlyLayerBlockViewModel.SelectedElement
        {
            get { return SelectedElement; }
        }

        public void AddInstance(ElementInstanceViewModel elementInstance)
        {
            instances.Add(elementInstance);
            uniqueIdentifierManager.Update(elementInstance.UniqueIdentifier);
        }

        public void RemoveInstance(ElementInstanceViewModel elementInstance)
        {
            instances.Remove(elementInstance);
        }
    }
}
