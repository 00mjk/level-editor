using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using LevelEditor.Controls;

namespace LevelEditor.Adorners
{
    public interface ISelectionContainer
    {
        bool HasSelection { get; }
        Rect SelectionBounds { get; }
        event EventHandler Render;
    }

    public class SelectedElementsAdorner : Adorner, ISelectionContainer
    {
        private ItemsControl adornedElement;

        private Pen selectionPen;

        public Rect SelectionBounds { get; private set; }

        public SelectedElementsAdorner(ItemsControl adornedElement)
            : base(adornedElement)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");

            this.adornedElement = adornedElement;
            IsHitTestVisible = false;

            selectionPen = new Pen(new SolidColorBrush(Color.FromArgb(128, 32, 64, 255)), 3.0);
            if (selectionPen.CanFreeze)
                selectionPen.Freeze();
        }

        private GeometryGroup selectionGeometry;

        public event EventHandler Render;

        protected override void OnRender(DrawingContext drawingContext)
        {
            isInvalidatingRender = false;

            UpdateForRender();

            base.OnRender(drawingContext);

            if (selectionGeometry != null)
                drawingContext.DrawGeometry(null, selectionPen, selectionGeometry);

            var handler = Render;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public bool HasSelection { get; private set; }

        private void UpdateForRender()
        {
            var icg = adornedElement.ItemContainerGenerator;

            var selectedContainers = adornedElement.Items
                .Cast<object>()
                .Select(x => icg.ContainerFromItem(x))
                .OfType<GameBoardItem>()
                .Where(x => x.IsSelected);

            selectionGeometry = new GeometryGroup();

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            var hasSelection = false;
            foreach (var selectedContainer in selectedContainers)
            {
                var rect = VisualTreeHelper.GetDescendantBounds(selectedContainer);
                var rectGeo = new RectangleGeometry(rect, 0.0, 0.0, selectedContainer.RenderTransform);

                var x = selectedContainer.UnitPositionX * adornedElement.ActualWidth;
                var y = selectedContainer.UnitPositionY * adornedElement.ActualHeight;
                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, x);
                maxY = Math.Max(maxY, y);

                selectionGeometry.Children.Add(rectGeo);

                hasSelection = true;
            }

            if (hasSelection)
                SelectionBounds = new Rect(minX, minY, maxX - minX, maxY - minY);
            else
                SelectionBounds = Rect.Empty;

            HasSelection = hasSelection;
        }

        private bool isInvalidatingRender;

        public void Update()
        {
            if (isInvalidatingRender)
                return;

            InvalidateVisual();

            isInvalidatingRender = true;
        }
    }
}
