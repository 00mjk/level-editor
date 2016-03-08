using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using LevelEditor.Extensibility;

namespace LevelEditor.Features.Selection
{
    public class StandardSelectionToolRenderer : ToolBoxToolRenderer
    {
        private Pen pen;
        private Brush brush;

        public StandardSelectionToolRenderer(UIElement adornedElement)
            : base(adornedElement)
        {
            IsHitTestVisible = false;

            byte r = 32;
            byte g = 64;
            byte b = 255;

            pen = new Pen(new SolidColorBrush(Color.FromArgb(128, r, g, b)), 1.0);
            if (pen.CanFreeze)
                pen.Freeze();

            brush = new SolidColorBrush(Color.FromArgb(64, r, g, b));
            if (brush.CanFreeze)
                brush.Freeze();
        }

        private bool isDrawing;
        private Rect rect;

        public void Draw(Rect rect)
        {
            this.rect = rect;
            isDrawing = true;
            InvalidateVisual();
        }

        public void EndDraw()
        {
            isDrawing = false;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (isDrawing == false)
                return;

            drawingContext.DrawRectangle(brush, pen, rect);
        }
    }
}
