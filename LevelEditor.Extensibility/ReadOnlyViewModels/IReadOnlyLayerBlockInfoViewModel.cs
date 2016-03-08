using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.UI.Core;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyLayerBlockInfoViewModel : IReadOnlyRootedViewModel
    {
        bool IsCanvasSizeMode { get; }

        double Zoom { get; }

        double BlockWidth { get; }
        double BlockHeight { get; }

        double SnapX { get; }
        double SnapY { get; }

        IReadOnlyObservableCollection<IReadOnlyFlagViewModel> Flags { get; }
    }
}
