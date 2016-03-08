using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Bitcraft.UI.Core;
using Bitcraft.UI.Core.Extensions;
using LayerDataReaderWriter;
using LevelEditor.Adorners;
using LevelEditor.Extensibility;
using LevelEditor.Features.Selection;
using Bitcraft.Core.Extensions;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LevelEditor.Controls
{
    public class CanvasForegroundRenderersAdorner : Adorner
    {
        public CanvasForegroundRenderersAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            IsHitTestVisible = false;
            ClipToBounds = false;
        }

        private IEnumerable<CanvasRendererWrapper> canvasForegroundRendererWrappers;

        public void SetCanvasForegroundRendererWrappers(IEnumerable<CanvasRendererWrapper> canvasForegroundRendererWrappers)
        {
            if (canvasForegroundRendererWrappers == null)
                canvasForegroundRendererWrappers = new CanvasRendererWrapper[0];

            this.canvasForegroundRendererWrappers = canvasForegroundRendererWrappers;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            foreach (var cfrw in canvasForegroundRendererWrappers.EmptyIfNull().Where(crw => crw.IsEnabled))
                cfrw.CanvasRenderer.Render(drawingContext);
        }
    }

    public class GameBoardItemsControl : ItemsControl
    {
        public GameBoardItemsControl()
        {
            canvasForegroundRenderersAdorner = new CanvasForegroundRenderersAdorner(this);

            this.Loaded += GameBoardItemsControl_Loaded;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            UpdateSelectionAdorner();
            UpdateTransformGizmosAdorner();
        }

        private CanvasForegroundRenderersAdorner canvasForegroundRenderersAdorner;

        private SelectedElementsAdorner selectedElementAdorner;
        private TransformGizmosAdorner transformGizmosAdorner;

        private void GameBoardItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            selectedElementAdorner = new SelectedElementsAdorner(this);
            transformGizmosAdorner = new TransformGizmosAdorner(this, selectedElementAdorner);

            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            adornerLayer.Add(selectedElementAdorner);
            adornerLayer.Add(transformGizmosAdorner);
            adornerLayer.Add(canvasForegroundRenderersAdorner);

            UpdateSelectionToolAdorner();

            var parent = Parent as FrameworkElement;
            // checking parent MouseDown event is to allow selecting from outside of workspace
            parent.MouseDown += parent_MouseDown;

            // avoid ScrollViewer to force scrolling this control when it gets focus
            this.RequestBringIntoView += (ss, ee) => ee.Handled = true;
        }

        public BitmapSource TakeScreenshot()
        {
            var dv = new DrawingVisual();
            var rect = new Rect(this.RenderSize);

            using (DrawingContext ctx = dv.RenderOpen())
            {
                ctx.DrawRectangle(Brushes.Transparent, null, rect); // background of the parent control
                ctx.DrawRectangle(new VisualBrush(this), null, rect);

                foreach (var cfrw in CanvasForegroundRendererWrappers.EmptyIfNull().Where(crw => crw.IsEnabled))
                    cfrw.CanvasRenderer.Render(ctx);
            }

            var rtb = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);
            rtb.Render(dv);

            return rtb;
        }

        public ICommand RequestScreenshotCommand
        {
            get { return (ICommand)GetValue(RequestScreenshotCommandProperty); }
            set { SetValue(RequestScreenshotCommandProperty, value); }
        }

        public static readonly DependencyProperty RequestScreenshotCommandProperty = DependencyProperty.Register(
            "RequestScreenshotCommand",
            typeof(ICommand),
            typeof(GameBoardItemsControl));

        public ICommand FuckTheOneWayToSourceCommand
        {
            get { return (ICommand)GetValue(FuckTheOneWayToSourceCommandProperty); }
            set { SetValue(FuckTheOneWayToSourceCommandProperty, value); }
        }

        public static readonly DependencyProperty FuckTheOneWayToSourceCommandProperty = DependencyProperty.Register(
            "FuckTheOneWayToSourceCommand",
            typeof(ICommand),
            typeof(GameBoardItemsControl),
            new PropertyMetadata(OnFuckTheOneWayToSourceCommandChanged));

        private static void OnFuckTheOneWayToSourceCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = (GameBoardItemsControl)sender;
            itemsControl.FuckTheOneWayToSourceCommand.ExecuteIfPossible(new ICommand[]
            {
                new AnonymousCommand(itemsControl.InvalidateVisual),
                new AnonymousCommand<IDictionary<string, BitmapSource>>(d => d["shot"] = itemsControl.TakeScreenshot())
            });
        }

        public IEnumerable<CanvasRendererWrapper> CanvasBackgroundRendererWrappers
        {
            get { return (IEnumerable<CanvasRendererWrapper>)GetValue(CanvasBackgroundRendererWrappersProperty); }
            set { SetValue(CanvasBackgroundRendererWrappersProperty, value); }
        }

        public static readonly DependencyProperty CanvasBackgroundRendererWrappersProperty = DependencyProperty.Register(
            "CanvasBackgroundRendererWrappers",
            typeof(IEnumerable<CanvasRendererWrapper>),
            typeof(GameBoardItemsControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public IEnumerable<CanvasRendererWrapper> CanvasForegroundRendererWrappers
        {
            get { return (IEnumerable<CanvasRendererWrapper>)GetValue(CanvasForegroundRendererWrappersProperty); }
            set { SetValue(CanvasForegroundRendererWrappersProperty, value); }
        }

        public static readonly DependencyProperty CanvasForegroundRendererWrappersProperty = DependencyProperty.Register(
            "CanvasForegroundRendererWrappers",
            typeof(IEnumerable<CanvasRendererWrapper>),
            typeof(GameBoardItemsControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnCanvasForegroundRendererWrappersChanged));

        private static void OnCanvasForegroundRendererWrappersChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = (GameBoardItemsControl)sender;
            itemsControl.canvasForegroundRenderersAdorner.SetCanvasForegroundRendererWrappers((IEnumerable<CanvasRendererWrapper>)e.NewValue);
        }

        private void parent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source != Parent || e.OriginalSource is Adorner)
                return;

            OnMouseDown(e);
            e.Handled = true;
        }

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register(
            "IsDragging",
            typeof(bool),
            typeof(GameBoardItemsControl),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnIsDraggingChanged));

        private static void OnIsDraggingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (GameBoardItemsControl)sender;
            ctrl.transformGizmosAdorner.IsHitTestVisible = !(bool)e.NewValue;
        }

        public IMouseTool SelectionTool
        {
            get { return (IMouseTool)GetValue(SelectionToolProperty); }
            set { SetValue(SelectionToolProperty, value); }
        }

        public static readonly DependencyProperty SelectionToolProperty = DependencyProperty.Register(
            "SelectionTool",
            typeof(IMouseTool),
            typeof(GameBoardItemsControl),
            new PropertyMetadata(OnSelectionToolChanged));

        private ToolBoxToolRenderer selectionToolRenderer;

        private void UpdateSelectionToolAdorner()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer == null)
                return;

            if (selectionToolRenderer != null)
                adornerLayer.Remove(selectionToolRenderer);

            if (SelectionTool == null)
                return;

            selectionToolRenderer = SelectionTool.GenerateRenderer(this);

            if (selectionToolRenderer != null)
                adornerLayer.Add(selectionToolRenderer);
        }

        private static void OnSelectionToolChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (GameBoardItemsControl)sender;
            var tool = (IMouseTool)e.NewValue;

            ctrl.UpdateSelectionToolAdorner();

            ctrl.UpdateSelectionAdorner();
            ctrl.UpdateTransformGizmosAdorner();
        }

        internal void BringToFront(GameBoardItem item)
        {
            foreach (var dataItem in Items)
            {
                var container = (GameBoardItem)ItemContainerGenerator.ContainerFromItem(dataItem);
                Canvas.SetZIndex(container, -1);
            }
            Canvas.SetZIndex(item, 0);
        }

        internal void UpdateSelectionAdorner()
        {
            if (selectedElementAdorner != null)
                selectedElementAdorner.Update();
        }

        internal void UpdateTransformGizmosAdorner()
        {
            if (transformGizmosAdorner != null)
                transformGizmosAdorner.Update();
        }

        internal void OnItemMouseDown(MouseButtonEventArgs e, object item)
        {
            this.Focus();

            Mouse.Capture(this);

            var position = e.GetPosition(this);
            if (SelectionTool != null)
                SelectionTool.MouseDown(position, e, item);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();

            Mouse.Capture(this);

            var position = e.GetPosition(this);
            if (SelectionTool != null)
                SelectionTool.MouseDown(position, e, null);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var position = e.GetPosition(this);
            if (SelectionTool != null)
                SelectionTool.MouseMove(position, e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            var position = e.GetPosition(this);
            if (SelectionTool != null)
                SelectionTool.MouseUp(position, e);

            Mouse.Capture(null);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (SelectionTool != null)
                SelectionTool.KeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (SelectionTool != null)
                SelectionTool.KeyUp(e);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            var containers = Items
                .Cast<object>()
                .Select(x => (GameBoardItem)ItemContainerGenerator.ContainerFromItem(x))
                .Where(x => x != null);

            foreach (var c in containers)
                c.UpdateTransform();

            UpdateSelectionAdorner();
            UpdateTransformGizmosAdorner();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GameBoardItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GameBoardItem(this);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            LevelEditor.Behaviors.DragDrop.SetDropTarget(element as UIElement, item as LevelEditor.Behaviors.IDropTarget);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            foreach (var cbrw in CanvasBackgroundRendererWrappers.EmptyIfNull().Where(crw => crw.IsEnabled))
                cbrw.CanvasRenderer.Render(drawingContext);

            base.OnRender(drawingContext);

            canvasForegroundRenderersAdorner.InvalidateVisual();
        }
    }
}
