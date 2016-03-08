using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using Bitcraft.UI.Core;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using LayerDataReaderWriter;
using System.Windows.Input;

namespace LevelEditor.BuiltinCanvasRenderers
{
    [Export(typeof(ICanvasRenderer))]
    public class ElementIdentifierCanvasRenderer : ViewModelBase, ICanvasRenderer, IDisposable
    {
        private Brush brush;

        public string DisplayName { get { return "Elements Identifier"; } }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (SetValue(ref isEnabled, value))
                    root.LayerData.RequestRender();
            }
        }

        private bool selectedOnly = false;
        public bool SelectedOnly
        {
            get { return selectedOnly; }
            set
            {
                if (SetValue(ref selectedOnly, value))
                    root.LayerData.RequestRender();
            }
        }

        private string types;
        public string Types
        {
            get { return types; }
            set
            {
                if (SetValue(ref types, value))
                {
                    SplitTypes();
                    root.LayerData.RequestRender();
                }
            }
        }

        private ushort[] splitTypes;

        private void SplitTypes()
        {
            if (string.IsNullOrWhiteSpace(types))
                splitTypes = null;
            else
            {
                var list = new List<ushort>();
                foreach (var t in types.Split(new[] { ',', ';', ' ', '\t', '-', ':', '+', '~' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    ushort value;
                    if (ushort.TryParse(t, out value))
                        list.Add(value);
                }
                splitTypes = list.ToArray();
            }
        }

        private Color color = Colors.White;
        public Color Color
        {
            get { return color; }
            set
            {
                if (SetValue(ref color, value))
                {
                    UpdatePen();
                    root.LayerData.RequestRender();
                }
            }
        }

        private void UpdatePen()
        {
            var colorWithAlpha = Color.FromArgb(92, Color.R, Color.G, Color.B);

            brush = new SolidColorBrush(colorWithAlpha);
            if (brush.CanFreeze)
                brush.Freeze();
        }

        public ElementIdentifierCanvasRenderer()
        {
            UpdatePen();
        }

        public void Dispose()
        {
            propertyChangedNotifier.PropertyChanged -= propertyChangedNotifier_PropertyChanged;
        }

        public RenderPlace RenderPlace
        {
            get { return RenderPlace.Foreground; }
        }

        private IReadOnlyRootViewModel root;
        private INotifyPropertyChanged propertyChangedNotifier;

        public static readonly RoutedCommand ElementIdentifierEnableCommand = new RoutedCommand(
            "ElementIdentifierEnableCommand",
            typeof(ElementIdentifierCanvasRenderer),
            new InputGestureCollection { new KeyGesture(Key.E, ModifierKeys.Control) });

        public void Initialize(IReadOnlyRootViewModel root, IGeometryHelper spriteHelper, INotifyPropertyChanged propertyChangedNotifier)
        {
            if (root == null)
                throw new ArgumentNullException("root");
            if (propertyChangedNotifier == null)
                throw new ArgumentNullException("propertyChangedNotifier");

            this.root = root;
            this.propertyChangedNotifier = propertyChangedNotifier;

            propertyChangedNotifier.PropertyChanged += propertyChangedNotifier_PropertyChanged;

            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(ElementIdentifierEnableCommand, (ss, ee) => IsEnabled = !IsEnabled));
        }

        private void propertyChangedNotifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UnitPositionX":
                case "UnitPositionY":
                case "PivotX":
                case "PivotY":
                case "SelectedElement":
                case "SelectedBlock":
                case "InstanceCount": // when deleted
                    root.LayerData.RequestRender();
                    break;
            }
        }

        public void Render(DrawingContext drawingContext)
        {
            if (!IsEnabled)
                return;

            var block = root.LayerData.SelectedBlock;
            if (block == null)
                return;

            var width = block.Info.BlockWidth;
            var height = block.Info.BlockHeight;

            foreach (var element in block.Instances)
            {
                if (splitTypes != null && splitTypes.Contains(element.Type) == false)
                    continue;

                if (SelectedOnly && element != root.LayerData.SelectedBlock.SelectedElement)
                    continue;

                const double size = 40.0;
                var center = new Point(element.UnitPositionX * width, element.UnitPositionY * height);
                var corner = new Point(center.X - size * 0.5, center.Y - size * 0.5);

                drawingContext.DrawRectangle(brush, null, new Rect(corner, new Size(size, size)));

                var text = new FormattedText(
                        element.UniqueIdentifier.ToString(),
                        CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Verdana"),
                        30,
                        Brushes.Black
                    );

                drawingContext.DrawText(text, new Point(corner.X + (size - text.Width) / 2.0, corner.Y + (size - text.Height) / 2.0));
            }
        }

        public bool HasConfiguration { get { return true; } }

        public void Configure()
        {
            var window = new ElementIdentifierConfigurationWindow(this);
            window.Show();
        }
    }
}
