using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerDataReaderWriter.V9;

namespace LevelEditor.Extensibility.ImportExport
{
    public interface ILayerDataExporter : ILayerDataIOExtension
    {
        void Export(string filename, LayerFile data);
    }
}
