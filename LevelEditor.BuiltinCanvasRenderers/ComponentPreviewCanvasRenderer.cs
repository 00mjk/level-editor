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
    public class ComponentPreviewCanvasRenderer : ViewModelBase, ICanvasRenderer, IDisposable
    {
        private Brush brush;
        private Pen pen;

        public string DisplayName { get { return "Component Preview"; } }

        private Color color = Colors.Black;
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
            var colorWithAlpha = Color.FromArgb((byte)(255.0), Color.R, Color.G, Color.B);

            brush = new SolidColorBrush(colorWithAlpha);
            if (brush.CanFreeze)
                brush.Freeze();

            pen = new Pen(brush, 10);
            if (pen.CanFreeze)
                pen.Freeze();
        }

        public ComponentPreviewCanvasRenderer()
        {
            UpdatePen();
        }

        public void Dispose()
        {
        }

        public bool HasConfiguration { get { return false; } }

        public void Configure()
        {
        }

        public RenderPlace RenderPlace
        {
            get { return RenderPlace.Foreground; }
        }

        private IReadOnlyRootViewModel root;
        private IGeometryHelper geometryHelper;

        public void Initialize(IReadOnlyRootViewModel root, IGeometryHelper geometryHelper, INotifyPropertyChanged propertyChangedNotifier)
        {
            if (root == null)
                throw new ArgumentNullException("root");
          
            this.root = root;
            this.geometryHelper = geometryHelper;

            if (outlinePen.CanFreeze)
                outlinePen.Freeze();

            propertyChangedNotifier.PropertyChanged += propertyChangedNotifier_PropertyChanged;
        }

        private void propertyChangedNotifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UnitPositionX":
                case "UnitPositionY":
                case "PivotX":
                case "PivotY":
                case "ScaleX":
                case "ScaleY":
                case "Angle":
                case "SelectedElement":
                case "SelectedBlock":
                case "Components":
                case "InstanceCount": // when deleted
                    root.LayerData.RequestRender();
                    break;
            }
        }

        private static readonly Typeface defaultTypeface = new Typeface(new FontFamily("arial"), FontStyles.Normal, FontWeights.Black, FontStretches.UltraExpanded);
        private static readonly Pen outlinePen = new Pen(Brushes.Black, 0.75);

        public void Render(DrawingContext drawingContext)
        {
            var block = root.LayerData.SelectedBlock;
            if (block == null)
                return;

            var blockWidth = block.Info.BlockWidth;
            var blockHeight = block.Info.BlockHeight;

            foreach (var element in block.Instances)
            {
                if (element.Components.Count == 0)
                    continue;

                const double size = 5.0;

                var unitSpaceRect = geometryHelper.GetAxisAlignedBoundingBox(block, element);
                var centerPoint = new Point(unitSpaceRect.Left + unitSpaceRect.Width / 2.0, unitSpaceRect.Top + unitSpaceRect.Height / 2.0);

                centerPoint.X *= blockWidth;
                centerPoint.Y *= blockHeight;

                drawingContext.DrawEllipse(brush, pen, centerPoint, size, size);

                var sb = new StringBuilder();

                foreach (var c in element.Components)
                    sb.AppendLine(c.ShortToolTipText);

                var text = new FormattedText(sb.ToString().TrimEnd(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, defaultTypeface, 16.0, Brushes.White);

                var geo = text.BuildGeometry(new Point(-text.Width / 2.0 + centerPoint.X, -text.Height / 2.0 + centerPoint.Y));
                drawingContext.DrawGeometry(Brushes.White, outlinePen, geo);
            }
        }
    }
}
