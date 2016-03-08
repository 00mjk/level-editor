using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerDataReaderWriter.V9;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyComponentInstancePropertyViewModel : IReadOnlyRootedViewModel
    {
        string Name { get; }
        ComponentPropertyType Type { get; }
        string Description { get; }
        bool IsPreset { get; }

        bool BooleanValue { get; }
        int IntegerValue { get; }
        float FloatValue { get; }
        string StringValue { get; }
        Guid GuidValue { get; }
    }
}
