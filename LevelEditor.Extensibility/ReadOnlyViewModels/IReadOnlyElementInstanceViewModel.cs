using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.UI.Core;
using LayerDataReaderWriter;
using LayerDataReaderWriter.V9;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyElementInstanceViewModel : IReadOnlyRootedViewModel
    {
        ushort UniqueIdentifier { get; }

        double UnitPositionX { get; }
        double UnitPositionY { get; }

        byte Layer { get; }

        double PivotX { get; }
        double PivotY { get; }

        double Angle { get; }

        double ScaleX { get; }
        double ScaleY { get; }

        double DpiBasedScaleX { get; }
        double DpiBasedScaleY { get; }

        bool IsSelected { get; }

        ushort Type { get; }
        ColliderType ColliderType { get; }

        IReadOnlyObservableCollection<IReadOnlyComponentInstanceViewModel> Components { get; }
    }
}
