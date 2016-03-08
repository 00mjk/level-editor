using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Bitcraft.UI.Core;
using LevelEditor.Extensibility.ImportExport;

namespace LevelEditor.ViewModels.ImportExport
{
    public class LayerDataExporterViewModel : LayerDataIOExtensionViewModelBase<ILayerDataExporter>
    {
        public LayerDataExporterViewModel(RootViewModel root, ILayerDataExporter exporter)
            : base(root, exporter)
        {
        }

        protected override void Run()
        {
            var layerBlocks = Root.LayerData.SaveData();

            try
            {
                dataModel.Export(Root.LayerData.FullFilename, layerBlocks);
            }
            catch (Exception ex)
            {
                var msg = new StringBuilder();
                msg.AppendLine(string.Format("Exporter '{0}' failed to execute", dataModel.DisplayName));
                msg.AppendLine();
                msg.Append(ex.ToString());
                MessageBox.Show(msg.ToString(), "Execution Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
