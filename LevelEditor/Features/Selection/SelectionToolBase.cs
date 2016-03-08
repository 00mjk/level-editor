using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Bitcraft.Core;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using LevelEditor.ViewModels;

namespace LevelEditor.Features.Selection
{
    public abstract class SelectionToolBase : IMouseTool
    {
        public LayerBlockViewModel Block { get; private set; }
        public ElementInstanceViewModel MouseDownItem { get; private set; }

        protected Point MouseDownPosition { get; private set; }
        protected Point LastPosition { get; private set; }

        protected ToolBoxToolRenderer Renderder { get; private set; }

        public void SetBlock(LayerBlockViewModel block)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            Block = block;
        }

        public void SetBlock(IReadOnlyLayerBlockViewModel block)
        {
            SetBlock(block as LayerBlockViewModel);
        }

        public void MouseDown(Point position, MouseButtonEventArgs e, object item)
        {
            MouseDownItem = item as ElementInstanceViewModel;
            MouseDownPosition = position;
            LastPosition = position;
            MouseDownOverride(position, e);
        }

        public void MouseMove(Point position, MouseEventArgs e)
        {
            LastPosition = position;
            MouseMoveOverride(position, e);
        }

        public void MouseUp(Point position, MouseButtonEventArgs e)
        {
            LastPosition = position;
            MouseUpOverride(position, e);
        }

        protected bool IsMouseDown { get; private set; }
        protected bool IsDragging { get; private set; }

        protected virtual void MouseDownOverride(Point position, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
            {
                IsMouseDown = true;
                IsDragging = false;

                if (MouseDownItem == null)
                {
                    if (IsControlKeyDown() == false)
                        ClearSelection();
                }
            }
        }

        protected virtual void MouseMoveOverride(Point position, MouseEventArgs e)
        {
            if (MouseDownItem == null)
                return;

            if (IsMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsDragging == false)
                {
                    if (CheckDrag(position))
                    {
                        IsDragging = true;
                        if (MouseDownItem.IsSelected == false)
                            PerformSelection();
                        BeginDragSelectedElements(MouseDownPosition);
                    }
                }
                else
                    DragSelectedElements(position);
            }
        }

        protected virtual void MouseUpOverride(Point position, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (IsDragging == false)
                    PerformSelection();
                else
                    EndDragSelectedElements();

                MouseDownItem = null;
                IsMouseDown = false;
                IsDragging = false;
            }
        }

        protected abstract void PerformSelection();

        protected Point ControlToUnit(Point p)
        {
            return new Point(p.X / Block.Info.BlockWidth, p.Y / Block.Info.BlockHeight);
        }

        protected Point UnitToControl(Point p)
        {
            return new Point(p.X * Block.Info.BlockWidth, p.Y * Block.Info.BlockHeight);
        }

        protected void ClearSelection()
        {
            foreach (var item in Block.Instances)
                item.IsSelected = false;
        }

        protected bool CheckDrag(Point position)
        {
            var dx = Math.Abs(position.X - MouseDownPosition.X);
            var dy = Math.Abs(position.Y - MouseDownPosition.Y);

            return dx > SystemParameters.MinimumHorizontalDragDistance || dy > SystemParameters.MinimumVerticalDragDistance;
        }

        private class Container
        {
            public ElementInstanceViewModel Element;
            public double X;
            public double Y;
        }

        private Container[] selectedItems;

        protected void BeginDragSelectedElements(Point mouseDownPosition)
        {
            selectedItems = Block.Instances
                .Where(n => n.IsSelected)
                .Select(n => new Container { Element = n, X = n.UnitPositionX, Y = n.UnitPositionY })
                .ToArray();
        }

        protected void DragSelectedElements(Point mousePosition)
        {
            if (selectedItems == null || selectedItems.Length == 0)
                return;

            double unitDeltaX = 0.0;
            double unitDeltaY = 0.0;

            if (isSnappingAlongY == false)
                unitDeltaX = (mousePosition.X - MouseDownPosition.X) / Block.Info.BlockWidth;

            if (isSnappingAlongX == false)
                unitDeltaY = (mousePosition.Y - MouseDownPosition.Y) / Block.Info.BlockHeight;

            var minUnitX = selectedItems.Min(item => item.X + unitDeltaX);
            if (minUnitX < 0.0)
                unitDeltaX -= minUnitX;
            var maxUnitX = selectedItems.Max(item => item.X + unitDeltaX);
            if (maxUnitX > 1.0)
                unitDeltaX += 1.0 - maxUnitX;
            var minUnitY = selectedItems.Min(item => item.Y + unitDeltaY);
            if (minUnitY < 0.0)
                unitDeltaY -= minUnitY;
            var maxUnitY = selectedItems.Max(item => item.Y + unitDeltaY);
            if (maxUnitY > 1.0)
                unitDeltaY += 1.0 - maxUnitY;

            var isSnappingToGrid = Block.Parent.IsSnappingToGrid;

            foreach (var item in selectedItems)
            {
                var newValueX = item.X + unitDeltaX;
                if (isSnappingToGrid)
                    newValueX = Block.ComputeSnapValueX(newValueX);
                item.Element.UnitPositionX = newValueX;

                var newValueY = item.Y + unitDeltaY;
                if (isSnappingToGrid)
                    newValueY = Block.ComputeSnapValueY(newValueY);
                item.Element.UnitPositionY = newValueY;
            }
        }

        protected void EndDragSelectedElements()
        {
        }

        protected bool IsControlKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        protected bool IsShiftKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        private bool IsAltKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
        }

        public ToolBoxToolRenderer GenerateRenderer(UIElement adornedElement)
        {
            var renderer = GenerateRendererOverride(adornedElement);
            Renderder = renderer;
            return renderer;
        }

        protected abstract ToolBoxToolRenderer GenerateRendererOverride(UIElement adornedElement);

        private bool isSnappingAlongX;
        private bool isSnappingAlongY;

        public void KeyDown(KeyEventArgs e)
        {
            if ((e.Key == Key.LeftShift || e.Key == Key.RightShift) && e.IsRepeat == false)
                Block.Root.LayerData.IsSnappingToGrid = false;

            if (IsControlKeyDown() == false)
            {
                if (e.Key == Key.Left)
                {
                    double dx = (Block.Root.LayerData.IsSnappingToGrid ? Block.Info.SnapX : 1.0f) / Block.Info.BlockWidth;
                    foreach (var item in Block.Instances.Where(n => n.IsSelected))
                        item.UnitPositionX -= dx;
                }
                else if (e.Key == Key.Right)
                {
                    double dx = (Block.Root.LayerData.IsSnappingToGrid ? Block.Info.SnapX : 1.0f) / Block.Info.BlockWidth;
                    using (var snapper = new Snapper(Block.Root))
                    {
                        Block.Root.LayerData.IsSnappingToGrid = false;
                        foreach (var item in Block.Instances.Where(n => n.IsSelected))
                            item.UnitPositionX += dx;
                    }
                }
                else if (e.Key == Key.Up)
                {
                    double dy = (Block.Root.LayerData.IsSnappingToGrid ? Block.Info.SnapY : 1.0f) / Block.Info.BlockHeight;
                    using (var snapper = new Snapper(Block.Root))
                    {
                        Block.Root.LayerData.IsSnappingToGrid = false;
                        foreach (var item in Block.Instances.Where(n => n.IsSelected))
                            item.UnitPositionY -= dy;
                    }
                }
                else if (e.Key == Key.Down)
                {
                    double dy = (Block.Root.LayerData.IsSnappingToGrid ? Block.Info.SnapY : 1.0f) / Block.Info.BlockHeight;
                    using (var snapper = new Snapper(Block.Root))
                    {
                        Block.Root.LayerData.IsSnappingToGrid = false;
                        foreach (var item in Block.Instances.Where(n => n.IsSelected))
                            item.UnitPositionY += dy;
                    }
                }
            }

            if (e.Key == Key.X && e.IsRepeat == false)
            {
                isSnappingAlongX = true;
                isSnappingAlongY = false;
            }
            else if (e.Key == Key.Y && e.IsRepeat == false)
            {
                isSnappingAlongX = false;
                isSnappingAlongY = true;
            }
        }

        public void KeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                Block.Root.LayerData.IsSnappingToGrid = true;
            if (e.Key == Key.X || e.Key == Key.Y)
            {
                isSnappingAlongX = false;
                isSnappingAlongY = false;
            }
        }
    }
}
