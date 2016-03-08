using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Bitcraft.UI.Core;
using LevelEditor.Extensibility.ImportExport;
using Bitcraft.Core;

namespace LevelEditor.ViewModels.ImportExport
{
    public class LayerDataImporterViewModel : LayerDataIOExtensionViewModelBase<ILayerDataImporter>
    {
        public LayerDataImporterViewModel(RootViewModel root, ILayerDataImporter importer)
            : base(root, importer)
        {
        }

        protected override async void Run()
        {
            var isCancelled = false;
            var cancellable = new AnonymousCancellable(c => isCancelled = c);

            // close only if there are modified data, so if the user cancels import bellow,
            // he/she will not have to reopen his/her previous level
            if (Root.IsDataModified)
            {
                Root.OnApplicationClose(cancellable);
                if (isCancelled)
                    return;
            }

            ImportResult importResult = null;

            try
            {
                importResult = await dataModel.Import();
            }
            catch (Exception ex)
            {
                var msg = new StringBuilder();
                msg.AppendLine(string.Format("Importer '{0}' failed to execute.", dataModel.DisplayName));
                msg.AppendLine();
                msg.Append(ex.ToString());
                MessageBox.Show(msg.ToString(), "Execution Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (importResult == null)
            {
                //var msg = string.Format("Importer '{0}' did not produce any value.", dataModel.DisplayName);
                //MessageBox.Show(msg, "Execution Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Root.ImportData(importResult.Filename, importResult.Data);
        }
    }
}
