using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerDataReaderWriter.V9;

namespace LevelEditor.Extensibility.ImportExport
{
    public class ImportResult
    {
        public string Filename { get; private set; }
        public LayerFile Data { get; private set; }

        public ImportResult(string filename, LayerFile data)
        {
            Filename = filename;
            Data = data;
        }
    }

    public interface ILayerDataImporter : ILayerDataIOExtension
    {
        Task<ImportResult> Import();
    }
}
