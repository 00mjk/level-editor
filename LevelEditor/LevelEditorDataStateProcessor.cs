using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bitcraft.UI.Core.Extensions;
using Bitcraft.UI.Core.DataStateManagement;
using LevelEditor.ViewModels;
using System.IO;
using LayerDataReaderWriter;
using LayerDataReaderWriter.V9;
using System.Windows;

namespace LevelEditor
{
    public class LevelEditorDataStateProcessor : IDataStateModelProcessor
    {
        private string filename;
        private bool isModified;

        private RootViewModel rootViewModel;

        public LevelEditorDataStateProcessor(RootViewModel rootViewModel)
        {
            if (rootViewModel == null)
                throw new ArgumentNullException("rootViewModel");

            this.rootViewModel = rootViewModel;
        }

        public void OnClose()
        {
            rootViewModel.LayerData.Clear();
        }

        public void OnLoad(Stream storage)
        {
            try
            {
                string fullFilename = null;

                if (storage is FileStream)
                    fullFilename = ((FileStream)storage).Name;

                rootViewModel.LayerData.LoadData(fullFilename, (LayerFile)ReaderWriterManager.Read(storage, Encoding.UTF8, App.CurrentFileFormatVersion));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                storage.Dispose();
            }
        }

        public void OnSave(Stream storage)
        {
            rootViewModel.SaveStatus = SaveStatus.Saving;

            try
            {
                ReaderWriterManager.Write(rootViewModel.LayerData.SaveData(), storage, Encoding.UTF8, App.CurrentFileFormatVersion);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                storage.Dispose();
            }
        }

        public void OnDataModified(bool isModified)
        {
            this.isModified = isModified;

            rootViewModel.UpdateTitle(filename, isModified);
        }

        public void OnNewData(Stream newStorage)
        {
            if (newStorage == null)
                filename = null;
            else
                filename = ((FileStream)newStorage).Name;

            rootViewModel.UpdateTitle(filename, isModified);
        }
    }
}
