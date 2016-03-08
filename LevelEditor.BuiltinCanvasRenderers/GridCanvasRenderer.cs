using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Bitcraft.UI.Core;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.BuiltinCanvasRenderers
{
    [Export(typeof(ICanvasRenderer))]
    public class GridCanvasRenderer : ViewModelBase, ICanvasRenderer, IDisposable
    {
        private Pen pen;

        public string DisplayName { get { return "Grid"; } }

        private double opacity = 1.0;
        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (SetValue(ref opacity, value))
                {
                    UpdatePen();
                    root.LayerData.RequestRender();
                }
            }
        }

        private double thickness = 0.6;
        public double Thickness
        {
            get { return thickness; }
            set
            {
                if (SetValue(ref thickness, value))
                {
                    UpdatePen();
                    root.LayerData.RequestRender();
                }
            }
        }

        private Color color = Colors.DimGray;
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
            var colorWithAlpha = Color.FromArgb((byte)(Opacity * 255.0), Color.R, Color.G, Color.B);

            var brush = new SolidColorBrush(colorWithAlpha);
            if (brush.CanFreeze)
                brush.Freeze();

            pen = new Pen(brush, Thickness);
            if (pen.CanFreeze)
                pen.Freeze();
        }

        public GridCanvasRenderer()
        {
            UpdatePen();
        }

        public void Dispose()
        {
            propertyChangedNotifier.PropertyChanged -= propertyChangedNotifier_PropertyChanged;
        }

        public RenderPlace RenderPlace
        {
            get { return RenderPlace.Background; }
        }

        private IReadOnlyRootViewModel root;
        private INotifyPropertyChanged propertyChangedNotifier;

        public void Initialize(IReadOnlyRootViewModel root, IGeometryHelper spriteHelper, INotifyPropertyChanged propertyChangedNotifier)
        {
            if (root == null)
                throw new ArgumentNullException("root");
            if (propertyChangedNotifier == null)
                throw new ArgumentNullException("propertyChangedNotifier");

            this.root = root;
            this.propertyChangedNotifier = propertyChangedNotifier;

            propertyChangedNotifier.PropertyChanged += propertyChangedNotifier_PropertyChanged;
        }

        private void propertyChangedNotifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BlockWidth":
                case "BlockHeight":
                case "SnapX":
                case "SnapY":
                    root.LayerData.RequestRender();
                    break;
            }
        }

        public void Render(DrawingContext drawingContext)
        {
            var block = root.LayerData.SelectedBlock;
            if (block == null)
                return;

            DrawGrid(drawingContext, block);
        }

        private void DrawGrid(DrawingContext dc, IReadOnlyLayerBlockViewModel block)
        {
            if (block == null)
                return;

            var width = block.Info.BlockWidth;
            var height = block.Info.BlockHeight;

            var snapX = block.Info.SnapX;
            var snapY = block.Info.SnapY;

            if (snapX / width < 0.005 || snapY / height < 0.005)
                return;

            double x;
            for (x = snapX; x < width; x += snapX)
                dc.DrawLine(pen, new Point(x, 0.0), new Point(x, height));

            double y;
            for (y = snapY; y < height; y += snapY)
                dc.DrawLine(pen, new Point(0.0, y), new Point(width, y));
        }

        public bool HasConfiguration { get { return true; } }

        public void Configure()
        {
            var window = new GridConfigurationWindow(this);
            window.Show();
        }
    }
}
