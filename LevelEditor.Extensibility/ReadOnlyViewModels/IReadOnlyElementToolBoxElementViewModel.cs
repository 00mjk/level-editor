using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerDataReaderWriter;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyElementToolBoxElementViewModel : IReadOnlyRootedViewModel
    {
        string IconFullPath { get; }
        ushort Type { get; }
        bool IsVisible { get; }
    }

    public interface IReadOnlyElementToolBoxCategoryViewModel : IReadOnlyRootedViewModel
    {
        string Name { get; }
        IEnumerable<IReadOnlyElementToolBoxElementViewModel> Elements { get; }
    }
}
