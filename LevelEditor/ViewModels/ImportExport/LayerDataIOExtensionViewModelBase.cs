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
    public abstract class LayerDataIOExtensionViewModelBase<TDataModel> : RootedViewModel where TDataModel : ILayerDataIOExtension
    {
        public virtual string DisplayText { get; private set; }
        public virtual bool HasConfiguration { get; private set; }

        protected TDataModel dataModel { get; private set; }

        public LayerDataIOExtensionViewModelBase(RootViewModel root, TDataModel dataModel)
            : base(root)
        {
            this.dataModel = dataModel;

            DisplayText = dataModel.DisplayName;
            HasConfiguration = dataModel.HasConfiguration;

            RunCommand = new AnonymousCommand(Run);

            if (HasConfiguration)
                ConfigureCommand = new AnonymousCommand(Configure);
        }

        public ICommand RunCommand { get; private set; }
        public ICommand ConfigureCommand { get; private set; }

        protected virtual void Configure()
        {
            if (HasConfiguration)
            {
                try
                {
                    dataModel.Configure();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), string.Format("Configuration Error", dataModel.DisplayName), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected abstract void Run();
    }
}
