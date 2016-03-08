using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.UI.Core;
using LayerDataReaderWriter;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyGroupedDoubleValueViewModel : IReadOnlyRootedViewModel
    {
        double Value { get; }
        bool HasExtendedValues { get; }

        double Min { get; }
        double Max { get; }
        double Avg { get; }
    }

    public interface IReadOnlySelectedBlockElementsViewModel : IReadOnlyRootedViewModel
    {
        ushort UniqueIdentifier { get; }
        IReadOnlyGroupedDoubleValueViewModel UnitPositionX { get; }
        IReadOnlyGroupedDoubleValueViewModel UnitPositionY { get; }
        IReadOnlyGroupedDoubleValueViewModel PivotX { get; }
        IReadOnlyGroupedDoubleValueViewModel PivotY { get; }
        IReadOnlyGroupedDoubleValueViewModel Angle { get; }
        IReadOnlyGroupedDoubleValueViewModel ScaleX { get; }
        IReadOnlyGroupedDoubleValueViewModel ScaleY { get; }
        byte Layer { get; }
        ushort Type { get; }
    }

    public interface IReadOnlyLayerBlockViewModel : IReadOnlyRootedViewModel
    {
        byte Difficulty { get; }
        byte Identifier { get; }
        bool IsEnabled { get; }
        IReadOnlyLayerBlockInfoViewModel Info { get; }
        IReadOnlySelectedBlockElementsViewModel SelectedValues { get; }
        IReadOnlyObservableCollection<IReadOnlyElementInstanceViewModel> Instances { get; }
        int InstanceCount { get; }
        bool IsDragging { get; }
        IReadOnlyElementInstanceViewModel SelectedElement { get; }
    }
}
