using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Extensibility.ReadOnlyViewModels
{
    public interface IReadOnlyRootViewModel : IReadOnlyViewModelBase
    {
        IReadOnlyLayerDataViewModel LayerData { get; }

        IReadOnlyComponentToolBoxViewModel ComponentToolBox { get; }
        IReadOnlyFeatureToolBoxViewModel FeatureToolBox { get; }
        IReadOnlyElementToolBoxViewModel ElementToolBox { get; }

        string Title { get; }
        bool IsDataModified { get; }
    }
}
