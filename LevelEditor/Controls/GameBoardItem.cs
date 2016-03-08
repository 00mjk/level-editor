using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LayerDataReaderWriter;

namespace LevelEditor.Controls
{
    public class GameBoardItem : ContentPresenter
    {
        private GameBoardItemsControl parent;

        static GameBoardItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GameBoardItem), new FrameworkPropertyMetadata(typeof(GameBoardItem)));
        }

        internal GameBoardItem(GameBoardItemsControl parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            this.parent = parent;
            this.Loaded += GameBoardItem_Loaded;
        }

        private void GameBoardItem_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateTransform();
        }

        public Point WorkspacePosition
        {
            get { return (Point)GetValue(WorkspacePositionProperty); }
            private set { SetValue(WorkspacePositionPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey WorkspacePositionPropertyKey = DependencyProperty.RegisterReadOnly(
            "WorkspacePosition",
            typeof(Point),
            typeof(GameBoardItem),
            new PropertyMetadata());

        public static readonly DependencyProperty WorkspacePositionProperty = WorkspacePositionPropertyKey.DependencyProperty;

        public double UnitPositionX
        {
            get { return (double)GetValue(UnitPositionXProperty); }
            set { SetValue(UnitPositionXProperty, value); }
        }

        public static readonly DependencyProperty UnitPositionXProperty = DependencyProperty.Register(
            "UnitPositionX",
            typeof(double),
            typeof(GameBoardItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTranslationChanged));

        public double UnitPositionY
        {
            get { return (double)GetValue(UnitPositionYProperty); }
            set { SetValue(UnitPositionYProperty, value); }
        }

        public static readonly DependencyProperty UnitPositionYProperty = DependencyProperty.Register(
            "UnitPositionY",
            typeof(double),
            typeof(GameBoardItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTranslationChanged));

        public double PivotX
        {
            get { return (double)GetValue(PivotXProperty); }
            set { SetValue(PivotXProperty, value); }
        }

        public static readonly DependencyProperty PivotXProperty = DependencyProperty.Register(
            "PivotX",
            typeof(double),
            typeof(GameBoardItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPivotChanged));

        public double PivotY
        {
            get { return (double)GetValue(PivotYProperty); }
            set { SetValue(PivotYProperty, value); }
        }

        public static readonly DependencyProperty PivotYProperty = DependencyProperty.Register(
            "PivotY",
            typeof(double),
            typeof(GameBoardItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPivotChanged));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle",
            typeof(double),
            typeof(GameBoardItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRotationChanged));

        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public static readonly DependencyProperty ScaleXProperty = DependencyProperty.Register(
            "ScaleX",
            typeof(double),
            typeof(GameBoardItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnScaleChanged));

        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        public static readonly DependencyProperty ScaleYProperty = DependencyProperty.Register(
            "ScaleY",
            typeof(double),
            typeof(GameBoardItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnScaleChanged));

        public double DpiBasedScaleX
        {
            get { return (double)GetValue(DpiBasedScaleXProperty); }
            set { SetValue(DpiBasedScaleXProperty, value); }
        }

        public static readonly DependencyProperty DpiBasedScaleXProperty = DependencyProperty.Register(
            "DpiBasedScaleX",
            typeof(double),
            typeof(GameBoardItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnScaleChanged));

        public double DpiBasedScaleY
        {
            get { return (double)GetValue(DpiBasedScaleYProperty); }
            set { SetValue(DpiBasedScaleYProperty, value); }
        }

        public static readonly DependencyProperty DpiBasedScaleYProperty = DependencyProperty.Register(
            "DpiBasedScaleY",
            typeof(double),
            typeof(GameBoardItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnScaleChanged));

        public byte Type
        {
            get { return (byte)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type",
            typeof(byte),
            typeof(GameBoardItem), new PropertyMetadata(OnTypeChanged));

        private static void OnTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = (GameBoardItem)sender;

            item.parent.UpdateSelectionAdorner();
            item.parent.UpdateTransformGizmosAdorner();
        }

        public byte Layer
        {
            get { return (byte)GetValue(LayerProperty); }
            set { SetValue(LayerProperty, value); }
        }

        public static readonly DependencyProperty LayerProperty = DependencyProperty.Register(
            "Layer",
            typeof(byte),
            typeof(GameBoardItem), new PropertyMetadata(OnDepthChanged));

        public byte Depth
        {
            get { return (byte)GetValue(DepthProperty); }
            set { SetValue(DepthProperty, value); }
        }

        public static readonly DependencyProperty DepthProperty = DependencyProperty.Register(
            "Depth",
            typeof(byte),
            typeof(GameBoardItem), new PropertyMetadata(OnDepthChanged));

        private static void OnDepthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = (GameBoardItem)sender;

            var z = item.Layer * 256 + item.Depth;
            Panel.SetZIndex(item, z);
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(GameBoardItem), new PropertyMetadata(OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = (GameBoardItem)sender;

            item.parent.UpdateSelectionAdorner();
            item.parent.UpdateTransformGizmosAdorner();
        }

        private static void OnTranslationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = (GameBoardItem)sender;

            item.UpdateTransform();

            item.parent.UpdateSelectionAdorner();
            item.parent.UpdateTransformGizmosAdorner();
        }

        private static void OnPivotChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = (GameBoardItem)sender;

            item.UpdateTransform();

            item.parent.UpdateSelectionAdorner();
            item.parent.UpdateTransformGizmosAdorner();
        }

        private static void OnRotationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = (GameBoardItem)sender;

            item.UpdateTransform();

            item.parent.UpdateSelectionAdorner();
        }

        private static void OnScaleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = (GameBoardItem)sender;

            item.UpdateTransform();

            item.parent.UpdateSelectionAdorner();
            item.parent.UpdateTransformGizmosAdorner();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            UpdateTransform();
            parent.UpdateSelectionAdorner();
            parent.UpdateTransformGizmosAdorner();
        }

        internal void UpdateTransform()
        {
            var px = UnitPositionX * parent.Width;
            var py = UnitPositionY * parent.Height;

            WorkspacePosition = new Point(px, py);

            var translate1 = new TranslateTransform(-ActualWidth * PivotX, -ActualHeight * (1.0 - PivotY));
            var scale = new ScaleTransform(ScaleX * DpiBasedScaleX, ScaleY * DpiBasedScaleY);
            var rotate = new RotateTransform(Angle);
            var translate2 = new TranslateTransform(px, py);

            var transform = new TransformGroup();
            transform.Children.Add(translate1);
            transform.Children.Add(scale);
            transform.Children.Add(rotate);
            transform.Children.Add(translate2);

            RenderTransform = transform;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            parent.OnItemMouseDown(e, DataContext);
            e.Handled = true;
        }
    }
}
