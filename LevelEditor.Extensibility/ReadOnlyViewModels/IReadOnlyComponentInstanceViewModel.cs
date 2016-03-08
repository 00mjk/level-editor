using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.UI.Core;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyComponentInstanceViewModel : IReadOnlyRootedViewModel
    {
        string Type { get; }
        string ShortToolTipText { get; }
        IReadOnlyObservableCollection<IReadOnlyComponentInstancePropertyViewModel> Properties { get; }
    }
}
