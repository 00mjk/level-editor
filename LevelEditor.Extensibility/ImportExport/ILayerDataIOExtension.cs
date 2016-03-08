using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Extensibility.ImportExport
{
    public interface ILayerDataIOExtension : IConfigurableExtension
    {
        string DisplayName { get; }
        void Initialize(string basePath);
    }
}
