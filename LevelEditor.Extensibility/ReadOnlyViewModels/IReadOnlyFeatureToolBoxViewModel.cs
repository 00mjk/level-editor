using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public enum ToolCategory
    {
        Selection,
    }

    public interface IReadOnlyToolBoxTool
    {
        Guid Id { get; }
        string Name { get; }
        ToolCategory Category { get; }
        IMouseTool Tool { get; }
    }

    public interface IReadOnlyFeatureToolBoxViewModel : IReadOnlyRootedViewModel
    {
        IReadOnlyToolBoxTool[] AvailableTools { get; }
        IReadOnlyToolBoxTool SelectedTool { get; }
    }
}
