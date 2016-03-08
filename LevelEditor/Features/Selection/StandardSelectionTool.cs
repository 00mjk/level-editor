using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LevelEditor.Extensibility;
using LevelEditor.ViewModels;

namespace LevelEditor.Features.Selection
{
    public class StandardSelectionTool : SelectionToolBase
    {
        private bool isMouseDown;
        private bool isDragging;

        protected override void MouseDownOverride(Point position, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
            {
                isMouseDown = true;
                isDragging = false;
            }
        }

        private class SelectionStatus
        {
            public ElementInstanceViewModel Element;
            public bool IsInSelectionRegion;
        }

        private SelectionStatus[] elementStatusContainers;

        protected override void MouseMoveOverride(Point position, MouseEventArgs e)
        {
            if (isMouseDown == false || e.LeftButton != MouseButtonState.Pressed)
                return;

            if (isDragging == false)
            {
                if (CheckDrag(position))
                {
                    isDragging = true;

                    if (MouseDownItem != null)
                    {
                        if (MouseDownItem.IsSelected == false)
                            PerformSelection();
                        BeginDragSelectedElements(MouseDownPosition);
                    }
                    else
                    {
                        if (IsControlKeyDown() == false)
                            ClearSelection();

                        elementStatusContainers = Block.Instances
                            .Select(x => new SelectionStatus
                                {
                                    Element = x,
                                    //OriginalSelectionState = x.IsSelected,
                                    IsInSelectionRegion = false
                                })
                            .ToArray();
                    }
                }
            }
            else
            {
                if (MouseDownItem != null)
                    DragSelectedElements(position);
                else
                {
                    var isCtrlDown = IsControlKeyDown();

                    var selectionRect = ComputeSelectionRect();

                    foreach (var c in elementStatusContainers)
                    {
                        var isInSelectionRegion = IsInSelectionRegion(c.Element, selectionRect);

                        if (isInSelectionRegion == c.IsInSelectionRegion)
                            continue;

                        c.IsInSelectionRegion = isInSelectionRegion;

                        if (isCtrlDown)
                            c.Element.IsSelected = !c.Element.IsSelected;
                        else
                        {
                            if (isInSelectionRegion)
                            {
                                // just entered
                                c.Element.IsSelected = true;
                            }
                            else
                            {
                                // just left
                                c.Element.IsSelected = false;
                            }
                        }
                    }

                    ((StandardSelectionToolRenderer)Renderder).Draw(selectionRect);
                }
            }
        }

        protected override void MouseUpOverride(Point position, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (isDragging == false)
                {
                    //PerformSelection();

                    if (MouseDownItem != null)
                    {
                        if (IsControlKeyDown())
                            MouseDownItem.IsSelected = !MouseDownItem.IsSelected;
                        else
                        {
                            ClearSelection();
                            MouseDownItem.IsSelected = true;
                        }
                    }
                    else
                    {
                        if (IsControlKeyDown() == false)
                            ClearSelection();
                    }
                }
                else
                {
                    EndDragSelectedElements();
                }

                isMouseDown = false;
                isDragging = false;

                ((StandardSelectionToolRenderer)Renderder).EndDraw();
            }
        }

        private Rect ComputeSelectionRect()
        {
            double minX;
            double maxX;
            double minY;
            double maxY;

            if (MouseDownPosition.X > LastPosition.X)
            {
                minX = LastPosition.X;
                maxX = MouseDownPosition.X;
            }
            else
            {
                minX = MouseDownPosition.X;
                maxX = LastPosition.X;
            }

            if (MouseDownPosition.Y > LastPosition.Y)
            {
                minY = LastPosition.Y;
                maxY = MouseDownPosition.Y;
            }
            else
            {
                minY = MouseDownPosition.Y;
                maxY = LastPosition.Y;
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private bool IsInSelectionRegion(ElementInstanceViewModel element, Rect selectionRect)
        {
            var p = new Point(
                element.UnitPositionX * Block.Info.BlockWidth,
                element.UnitPositionY * Block.Info.BlockHeight);

            return selectionRect.Contains(p);
        }

        protected override void PerformSelection()
        {
            if (MouseDownItem == null)
                return;

            if (IsControlKeyDown())
                MouseDownItem.IsSelected = !MouseDownItem.IsSelected;
            else
            {
                ClearSelection();
                MouseDownItem.IsSelected = true;
            }

            // TODO: parent.BringToFront(this);
        }

        protected override ToolBoxToolRenderer GenerateRendererOverride(UIElement adornedElement)
        {
            return new StandardSelectionToolRenderer(adornedElement);
        }
    }
}
