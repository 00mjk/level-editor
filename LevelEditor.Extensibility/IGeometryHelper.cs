using System;
using System.Windows;
using System.Windows.Media;
using LayerDataReaderWriter.V9;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.Extensibility
{
    public interface IGeometryHelper
    {
        Size GetSpriteSize(LayerBlockElement element);
        Size GetSpriteSize(IReadOnlyElementInstanceViewModel element);

        Rect GetAxisAlignedBoundingBox(LayerBlock block, LayerBlockElement element);
        Rect GetAxisAlignedBoundingBox(IReadOnlyLayerBlockViewModel block, IReadOnlyElementInstanceViewModel element);

        Transform GetElementTransform(LayerBlock block, LayerBlockElement element);
        Transform GetElementTransform(IReadOnlyLayerBlockViewModel block, IReadOnlyElementInstanceViewModel element);

        bool AreColliding(LayerBlock block, LayerBlockElement element1, LayerBlockElement element2);
        bool AreColliding(IReadOnlyLayerBlockViewModel block, IReadOnlyElementInstanceViewModel element1, IReadOnlyElementInstanceViewModel element2);

        bool AreColliding(LayerBlock block, LayerBlockElement element, Point linePoint1, Point linePoint2);
        bool AreColliding(IReadOnlyLayerBlockViewModel block, IReadOnlyElementInstanceViewModel element, Point linePoint1, Point linePoint2);

        bool AreColliding(Geometry geo1, Geometry geo2);
    }
}
