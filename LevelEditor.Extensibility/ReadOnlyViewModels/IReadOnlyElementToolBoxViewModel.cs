using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyElementToolBoxViewModel : IReadOnlyRootedViewModel
    {
        IEnumerable<IReadOnlyElementToolBoxCategoryViewModel> Categories { get; }
        string SearchText { get; }
    }
}
