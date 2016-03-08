using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LevelEditor.Behaviors
{
    public interface IDragSource
    {
        /// <summary>
        /// Called when begining a drag operation.
        /// </summary>
        /// <param name="position">The mouse position where drag operation began.</param>
        /// <returns>Returns the allowed drag and drop operations.</returns>
        DragDropEffects BeginDrag(Point position);
    }

    public enum DropTargetEventType
    {
        Enter,
        Over,
        Leave,
    }

    public interface IDropTarget
    {
        DragDropEffects Drag(DropTargetEventType type, IDataObject data, Point position);
        void Drop(IDataObject data, Point position);
    }

    public class DragDrop
    {
        public static DependencyProperty DragSourceProperty = DependencyProperty.RegisterAttached(
            "DragSource",
            typeof(IDragSource),
            typeof(DragDrop),
            new PropertyMetadata(OnDragSourceChanged));

        public static IDragSource GetDragSource(UIElement target)
        {
            return (IDragSource)target.GetValue(DragSourceProperty);
        }

        public static void SetDragSource(UIElement target, IDragSource value)
        {
            target.SetValue(DragSourceProperty, value);
        }

        private static void OnDragSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)sender;

            if (e.NewValue == null)
            {
                element.MouseDown -= element_MouseDown;
                element.MouseMove -= element_MouseMove;
            }
            else
            {
                element.MouseDown += element_MouseDown;
                element.MouseMove += element_MouseMove;
            }
        }

        private class MouseDragInfo
        {
            public Point mouseDownPosition;
            public bool isMouseDown;
            public bool isDragging;
        }

        private static DependencyProperty mouseDragInfoProperty = DependencyProperty.RegisterAttached("MouseDragInfo", typeof(MouseDragInfo), typeof(DragDrop));

        private static void element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;

            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
            {
                var mouseDragInfo = new MouseDragInfo
                {
                    isMouseDown = true,
                    isDragging = false,
                    mouseDownPosition = e.GetPosition(null)
                };
                element.SetValue(mouseDragInfoProperty, mouseDragInfo);
            }
        }

        private static void element_MouseMove(object sender, MouseEventArgs e)
        {
            var element = (UIElement)sender;

            object value = element.GetValue(mouseDragInfoProperty);
            if (value == null || value == DependencyProperty.UnsetValue)
                return;

            IDragSource dragSource = GetDragSource(element);
            if (dragSource == null)
                return;

            var mouseDragInfo = (MouseDragInfo)value;

            if (mouseDragInfo.isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                if (mouseDragInfo.isDragging == false)
                {
                    var position = e.GetPosition(null);

                    var dx = Math.Abs(mouseDragInfo.mouseDownPosition.X - position.X);
                    var dy = Math.Abs(mouseDragInfo.mouseDownPosition.Y - position.Y);

                    if (dx > SystemParameters.MinimumHorizontalDragDistance || dy > SystemParameters.MinimumVerticalDragDistance)
                    {
                        mouseDragInfo.isDragging = true;
                        var allowedEffects = dragSource.BeginDrag(mouseDragInfo.mouseDownPosition);
                        if (allowedEffects != DragDropEffects.None)
                            System.Windows.DragDrop.DoDragDrop(element, dragSource, allowedEffects);
                        mouseDragInfo.isDragging = false;
                    }
                }
            }
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

        public static DependencyProperty DropTargetProperty = DependencyProperty.RegisterAttached(
            "DropTarget",
            typeof(IDropTarget),
            typeof(DragDrop),
            new PropertyMetadata(OnDropTargetChanged));

        public static IDropTarget GetDropTarget(UIElement target)
        {
            return (IDropTarget)target.GetValue(DropTargetProperty);
        }

        public static void SetDropTarget(UIElement target, IDropTarget value)
        {
            target.SetValue(DropTargetProperty, value);
        }

        private static void OnDropTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)sender;

            if (e.OldValue != null)
            {
                element.AllowDrop = false;
                element.DragEnter -= element_DragEnter;
                element.DragLeave -= element_DragLeave;
                element.DragOver -= element_DragOver;
                element.Drop -= element_Drop;
            }
            
            if (e.NewValue != null)
            {
                element.DragEnter += element_DragEnter;
                element.DragLeave += element_DragLeave;
                element.DragOver += element_DragOver;
                element.Drop += element_Drop;
                element.AllowDrop = true;
            }
        }

        private static void element_DragEnter(object sender, DragEventArgs e)
        {
            var element = (UIElement)sender;

            IDropTarget dropTarget = GetDropTarget(element);
            if (dropTarget != null)
            {
                e.Effects = dropTarget.Drag(DropTargetEventType.Enter, e.Data, e.GetPosition(element));
                e.Handled = true;
            }
        }

        private static void element_DragLeave(object sender, DragEventArgs e)
        {
            var element = (UIElement)sender;

            IDropTarget dropTarget = GetDropTarget(element);
            if (dropTarget != null)
            {
                e.Effects = dropTarget.Drag(DropTargetEventType.Leave, e.Data, e.GetPosition(element));
                e.Handled = true;
            }
        }

        private static void element_DragOver(object sender, DragEventArgs e)
        {
            var element = (UIElement)sender;

            IDropTarget dropTarget = GetDropTarget(element);
            if (dropTarget != null)
            {
                e.Effects = dropTarget.Drag(DropTargetEventType.Over, e.Data, e.GetPosition(element));
                e.Handled = true;
            }
        }

        private static void element_Drop(object sender, DragEventArgs e)
        {
            var element = (UIElement)sender;

            IDropTarget dropTarget = GetDropTarget(element);
            if (dropTarget != null)
            {
                dropTarget.Drop(e.Data, e.GetPosition(element));
                e.Handled = true;
            }
        }
    }
}
