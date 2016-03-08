using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyComponentToolBoxViewModel : IReadOnlyRootedViewModel
    {
        IReadOnlyComponentViewModel[] Components { get; }
        IReadOnlyComponentViewModel GetComponentFromType(string type);
    }
}
