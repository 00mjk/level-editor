using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.UI.Core;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyLayerDataViewModel : IReadOnlyRootedViewModel
    {
        string FullFilename { get; }
        bool WorkInProgress { get; }
        string WorkInProgressText { get; }
        double WorkInProgressRate { get; }
        IReadOnlyLayerBlockViewModel SelectedBlock { get; }
        IMouseTool SelectionTool { get; }
        IReadOnlyObservableCollection<IReadOnlyLayerBlockViewModel> Blocks { get; }
        bool MoveBlockUp(IReadOnlyLayerBlockViewModel block);
        bool MoveBlockDown(IReadOnlyLayerBlockViewModel block);
        void RequestRender();
    }
}
