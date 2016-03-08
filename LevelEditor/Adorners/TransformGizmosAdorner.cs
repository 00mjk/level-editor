using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using LevelEditor.Controls;

namespace LevelEditor.Adorners
{
    public class TransformGizmosAdorner : Adorner, IDisposable
    {
        private ItemsControl adornedElement;

        private Pen rotatePen;
        private Pen rotatePenOver;
        private Pen rotatePenOverDown;
        private Brush rotateBrush;

        private Pen pivotPen;

        //////////private Pen scalePen;
        //////////private Pen scalePenOver;
        //////////private Pen scalePenOverDown;
        //////////private Brush scaleBrush;
        //////////private Brush scaleBrushOver;
        //////////private Brush scaleBrushOverDown;

        private ISelectionContainer selectionContainer;

        public TransformGizmosAdorner(ItemsControl adornedElement, ISelectionContainer selectionContainer)
            : base(adornedElement)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            if (selectionContainer == null)
                throw new ArgumentNullException("selectionContainer");

            this.adornedElement = adornedElement;
            this.selectionContainer = selectionContainer;

            selectionContainer.Render += selectionContainer_Render;

            // ---------------------------------------------------------------------------

            rotateBrush = new SolidColorBrush(Color.FromArgb(192, 255, 255, 0));
            if (rotateBrush.CanFreeze)
                rotateBrush.Freeze();

            rotatePen = new Pen(new SolidColorBrush(Color.FromArgb(64, 255, 255, 0)), 11.0);
            if (rotatePen.CanFreeze)
                rotatePen.Freeze();

            rotatePenOver = new Pen(new SolidColorBrush(Color.FromArgb(128, 255, 255, 0)), 11.0);
            if (rotatePenOver.CanFreeze)
                rotatePenOver.Freeze();

            rotatePenOverDown = new Pen(new SolidColorBrush(Color.FromArgb(192, 255, 255, 0)), 11.0);
            if (rotatePenOverDown.CanFreeze)
                rotatePenOverDown.Freeze();

            // ---------------------------------------------------------------------------

            pivotPen = new Pen(Brushes.Red, 1.0);
            if (pivotPen.CanFreeze)
                pivotPen.Freeze();

            // ---------------------------------------------------------------------------

            //////////byte r = 0;
            //////////byte g = 0;
            //////////byte b = 255;
            //////////
            //////////scalePen = new Pen(new SolidColorBrush(Color.FromArgb(64, r, g, b)), 7.0);
            //////////if (scalePen.CanFreeze)
            //////////    scalePen.Freeze();
            //////////
            //////////scalePenOver = new Pen(new SolidColorBrush(Color.FromArgb(128, r, g, b)), 7.0);
            //////////if (scalePenOver.CanFreeze)
            //////////    scalePenOver.Freeze();
            //////////
            //////////scalePenOverDown = new Pen(new SolidColorBrush(Color.FromArgb(192, r, g, b)), 7.0);
            //////////if (scalePenOverDown.CanFreeze)
            //////////    scalePenOverDown.Freeze();
            //////////
            //////////scaleBrush = new SolidColorBrush(Color.FromArgb(32, r, g, b));
            //////////if (scaleBrush.CanFreeze)
            //////////    scaleBrush.Freeze();
            //////////
            //////////scaleBrushOver = new SolidColorBrush(Color.FromArgb(64, r, g, b));
            //////////if (scaleBrushOver.CanFreeze)
            //////////    scaleBrushOver.Freeze();
            //////////
            //////////scaleBrushOverDown = new SolidColorBrush(Color.FromArgb(128, r, g, b));
            //////////if (scaleBrushOverDown.CanFreeze)
            //////////    scaleBrushOverDown.Freeze();
        }

        void selectionContainer_Render(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private Point rotationCenter;

        private const double MinimumRadius = 15.0;

        public void Update()
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (selectionContainer.HasSelection == false)
                return;

            rotationCenter = new Point(
                selectionContainer.SelectionBounds.X + selectionContainer.SelectionBounds.Width / 2.0,
                selectionContainer.SelectionBounds.Y + selectionContainer.SelectionBounds.Height / 2.0);

            Pen localRotatePen;

            if (isOver == false)
                localRotatePen = rotatePen;
            else
            {
                if (isMouseDown == false)
                    localRotatePen = rotatePenOver;
                else
                    localRotatePen = rotatePenOverDown;
            }

            const double radius = 50.0;
            const double pivotSize = 10.0;

            //var pathFigure = new PathFigure();

            //pathFigure.StartPoint = rotationCenter;
            //pathFigure.IsClosed = true;
            //pathFigure.IsFilled = true;

            //var startVec = new Vector(mouseDownPosition.X - rotationCenter.X, mouseDownPosition.Y - rotationCenter.Y);
            //startVec.Normalize();
            //startVec *= (radius - 7.0);

            //var endVec = new Vector(currentMousePosition.X - rotationCenter.X, currentMousePosition.Y - rotationCenter.Y);
            //endVec.Normalize();
            //endVec *= (radius - 7.0);

            //pathFigure.Segments.Add(new LineSegment(new Point(rotationCenter.X + startVec.X, rotationCenter.Y + startVec.Y), false));

            //pathFigure.Segments.Add(
            //    new ArcSegment(
            //        new Point(rotationCenter.X + endVec.X, rotationCenter.Y + endVec.Y),
            //        new Size(radius, radius),
            //        deltaAngle,
            //        Math.Abs(deltaAngle) >= 180.0,
            //        SweepDirection.Counterclockwise,
            //        false));
            //drawingContext.DrawGeometry(rotateBrush, null, new PathGeometry(new [] { pathFigure }));

            drawingContext.DrawEllipse(null, localRotatePen, rotationCenter, radius, radius);

            drawingContext.DrawLine(pivotPen, new Point(rotationCenter.X - pivotSize / 2.0, rotationCenter.Y), new Point(rotationCenter.X + pivotSize / 2.0, rotationCenter.Y));
            drawingContext.DrawLine(pivotPen, new Point(rotationCenter.X, rotationCenter.Y - pivotSize / 2.0), new Point(rotationCenter.X, rotationCenter.Y + pivotSize / 2.0));

            //drawingContext.DrawText(new FormattedText(deltaAngle.ToString(), System.Globalization.CultureInfo.InvariantCulture, System.Windows.FlowDirection.LeftToRight, new Typeface("Arial"), 14.0, Brushes.White), rotationCenter);

            //////////drawingContext.DrawRoundedRectangle(scaleBrush, scalePen, new Rect(rotationCenter.X - 12.0, rotationCenter.Y - 90.0, 24.0, 24.0), 4.0, 4.0);
            //////////drawingContext.DrawRoundedRectangle(scaleBrush, scalePen, new Rect(rotationCenter.X + 66.0, rotationCenter.Y - 12.0, 24.0, 24.0), 4.0, 4.0);
        }

        private bool isOver;
        private bool isMouseDown;
        private Point mouseDownPosition;
        private double mouseAngleMouseDown;

        private class Container
        {
            public GameBoardItem Item;
            public double Angle;
        }

        private Container[] selectedContainers;

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            isOver = true;
            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            isOver = false;
            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
            {
                mouseDownPosition = e.GetPosition(adornedElement);
                mouseAngleMouseDown = ComputeAngle(mouseDownPosition);

                if (double.IsNaN(mouseAngleMouseDown) == false)
                {
                    e.MouseDevice.Capture(this);
                    isMouseDown = true;

                    var icg = adornedElement.ItemContainerGenerator;
                    selectedContainers = adornedElement.Items
                        .Cast<object>()
                        .Select(x => icg.ContainerFromItem(x))
                        .OfType<GameBoardItem>()
                        .Where(x => x.IsSelected)
                        .Select(x => new Container { Item = x, Angle = x.Angle })
                        .ToArray();

                    OnMouseMove(e);
                    InvalidateVisual();
                }
            }
        }

        private Point currentMousePosition;
        private double deltaAngle;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                currentMousePosition = e.GetPosition(adornedElement);

                var angle = ComputeAngle(currentMousePosition);
                if (double.IsNaN(angle) == false)
                {
                    var localDeltaAngle = (mouseAngleMouseDown - angle) * 180.0 / Math.PI;

                    var isShiftDown = Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftShift);
                    var isCtrlDown = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);

                    deltaAngle = localDeltaAngle;
                    if (isShiftDown)
                        deltaAngle = SnapAngle(deltaAngle);

                    foreach (var selected in selectedContainers)
                    {
                        if (isCtrlDown)
                            selected.Item.Angle = deltaAngle;
                        else
                        {
                            var temp = selected.Angle + localDeltaAngle;
                            if (isShiftDown)
                                temp = SnapAngle(temp);
                            selected.Item.Angle = temp;
                        }
                    }

                    InvalidateVisual();
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
            {
                e.MouseDevice.Capture(null);
                isMouseDown = false;
                InvalidateVisual();
            }
        }

        private const double SnapAngleDegrees = 15.0;

        private double SnapAngle(double angle)
        {
            return Math.Floor(angle / SnapAngleDegrees) * SnapAngleDegrees;
        }

        private double ComputeAngle(Point position)
        {
            var dx = position.X - rotationCenter.X;
            var dy = position.Y - rotationCenter.Y;

            var distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance < MinimumRadius)
                return double.NaN;

            var angle = -Math.Asin(dy / distance);
            if (dx < 0.0)
                angle = -angle + Math.PI;

            return angle;
        }

        public void Dispose()
        {
            selectionContainer.Render -= selectionContainer_Render;
        }
    }
}
