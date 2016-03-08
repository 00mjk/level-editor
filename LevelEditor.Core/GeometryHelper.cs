using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using LayerDataReaderWriter.V9;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.Core
{
    public class GeometryHelper : IGeometryHelper
    {
        public Size GetSpriteSize(LayerBlockElement element)
        {
            Size result;
            ImageManager.GetImageSize(element.Type, out result);
            return result;
        }

        public Size GetSpriteSize(IReadOnlyElementInstanceViewModel element)
        {
            Size result;
            ImageManager.GetImageSize(element.Type, out result);
            return result;
        }

        // =============================================================================================

        public Rect GetAxisAlignedBoundingBox(LayerBlock block, LayerBlockElement element)
        {
            var rect = new Rect(GetSpriteSize(element));
            var transform = GetElementTransform(block, element);

            rect = transform.TransformBounds(rect); // transform rect
            rect.Scale(1.0 / block.Width, 1.0 / block.Height); // change to unit space

            return rect;
        }

        public Rect GetAxisAlignedBoundingBox(IReadOnlyLayerBlockViewModel block, IReadOnlyElementInstanceViewModel element)
        {
            var rect = new Rect(GetSpriteSize(element));
            var transform = GetElementTransform(block, element);

            rect = transform.TransformBounds(rect); // transform rect
            rect.Scale(1.0 / block.Info.BlockWidth, 1.0 / block.Info.BlockHeight); // change to unit space

            return rect;
        }

        // =============================================================================================

        public bool AreColliding(LayerBlock block, LayerBlockElement element1, LayerBlockElement element2)
        {
            var elemTransform1 = GetElementTransform(block, element1);
            var rectGeo1 = new RectangleGeometry(new Rect(GetSpriteSize(element1)), 0.0, 0.0, elemTransform1);

            var elemTransform2 = GetElementTransform(block, element2);
            var rectGeo2 = new RectangleGeometry(new Rect(GetSpriteSize(element2)), 0.0, 0.0, elemTransform2);

            return AreColliding(rectGeo1, rectGeo2);
        }

        public bool AreColliding(IReadOnlyLayerBlockViewModel block, IReadOnlyElementInstanceViewModel element1, IReadOnlyElementInstanceViewModel element2)
        {
            var elemTransform1 = GetElementTransform(block, element1);
            var rectGeo1 = new RectangleGeometry(new Rect(GetSpriteSize(element1)), 0.0, 0.0, elemTransform1);

            var elemTransform2 = GetElementTransform(block, element2);
            var rectGeo2 = new RectangleGeometry(new Rect(GetSpriteSize(element2)), 0.0, 0.0, elemTransform2);

            return AreColliding(rectGeo1, rectGeo2);
        }

        // =============================================================================================

        public bool AreColliding(LayerBlock block, LayerBlockElement element, Point linePoint1, Point linePoint2)
        {
            var elemTransform = GetElementTransform(block, element);
            var rectGeo = new RectangleGeometry(new Rect(GetSpriteSize(element)), 0.0, 0.0, elemTransform);

            var lineGeo = new LineGeometry(
                new Point(linePoint1.X * block.Width, linePoint1.Y * block.Height),
                new Point(linePoint2.X * block.Width, linePoint2.Y * block.Height));

            return AreColliding(rectGeo, lineGeo);
        }

        public bool AreColliding(IReadOnlyLayerBlockViewModel block, IReadOnlyElementInstanceViewModel element, Point linePoint1, Point linePoint2)
        {
            var elemTransform = GetElementTransform(block, element);
            var rectGeo = new RectangleGeometry(new Rect(GetSpriteSize(element)), 0.0, 0.0, elemTransform);

            var lineGeo = new LineGeometry(
                new Point(linePoint1.X * block.Info.BlockWidth, linePoint1.Y * block.Info.BlockHeight),
                new Point(linePoint2.X * block.Info.BlockWidth, linePoint2.Y * block.Info.BlockHeight));

            return AreColliding(rectGeo, lineGeo);
        }

        // =============================================================================================

        public bool AreColliding(Geometry geo1, Geometry geo2)
        {
            var check = geo1.FillContainsWithDetail(geo2, 1e-3, ToleranceType.Absolute);

            return check == IntersectionDetail.Intersects ||
                check == IntersectionDetail.FullyContains ||
                check == IntersectionDetail.FullyInside;
        }

        // =============================================================================================

        public Transform GetElementTransform(LayerBlock block, LayerBlockElement element)
        {
            return GetElementTransform(
                block.Width, block.Height,
                element.Tx, element.Ty,
                element.Px, element.Py,
                element.Sx, element.Sy,
                element.Angle,
                element.Type);
        }

        public Transform GetElementTransform(IReadOnlyLayerBlockViewModel block, IReadOnlyElementInstanceViewModel element)
        {
            return GetElementTransform(
                block.Info.BlockWidth, block.Info.BlockHeight,
                element.UnitPositionX, element.UnitPositionY,
                element.PivotX, element.PivotY,
                element.ScaleX, element.ScaleY,
                element.Angle,
                element.Type);
        }

        private Transform GetElementTransform(
            double blockWidth, double blockHeight,
            double elementTx, double elementTy,
            double elementPx, double elementPy,
            double elementSx, double elementSy,
            double elementAngle,
            ushort elementType)
        {
            Size spriteSize;
            if (ImageManager.GetImageSize(elementType, out spriteSize) == false)
                return Transform.Identity;

            double dpiX;
            double dpiY;
            double outputDpiX;
            double outputDpiY;
            if (ImageManager.GetImageDpi(elementType, out dpiX, out dpiY, out outputDpiX, out outputDpiY) == false)
                return Transform.Identity;

            dpiX /= outputDpiX;
            dpiY /= outputDpiY;

            var translate1 = new TranslateTransform(-spriteSize.Width * elementPx, -spriteSize.Height * (1.0 - elementPy));
            var scale = new ScaleTransform(elementSx * dpiX, elementSy * dpiY);
            var rotate = new RotateTransform(elementAngle);
            var translate2 = new TranslateTransform(elementTx * blockWidth, elementTy * blockHeight);

            var transform = new TransformGroup();
            transform.Children.Add(translate1);
            transform.Children.Add(scale);
            transform.Children.Add(rotate);
            transform.Children.Add(translate2);

            return transform;
        }
    }
}
