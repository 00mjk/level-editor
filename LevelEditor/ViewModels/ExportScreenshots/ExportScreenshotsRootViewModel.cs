using Bitcraft.Core.Interfaces;
using Bitcraft.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LevelEditor.ViewModels.ExportScreenshots
{
    public class ExportScreenshotsRootViewModel : RootedViewModel
    {
        public ExportLocationViewModel ExportLocation { get; private set; }
        public ExportConditionsViewModel ExportConditions { get; private set; }
        public ExportResolutionViewModel ExportResolution { get; private set; }
        public ExportStripViewModel ExportStrip { get; private set; }
        public ExportInformationViewModel ExportInformation { get; private set; }

        private ICommand acceptCommand;
        public ICommand AcceptCommand
        {
            get { return acceptCommand; }
            private set { SetValue(ref acceptCommand, value); }
        }

        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            private set { SetValue(ref cancelCommand, value); }
        }

        private Action accept;

        public ExportScreenshotsRootViewModel(RootViewModel root)
            : base(root)
        {
            ExportLocation = new ExportLocationViewModel(root);
            ExportConditions = new ExportConditionsViewModel(root);
            ExportResolution = new ExportResolutionViewModel(root);
            ExportStrip = new ExportStripViewModel(root);
            ExportInformation = new ExportInformationViewModel(root);
        }

        public void SetWindowControls(Action accept, Action close)
        {
            this.accept = accept;

            AcceptCommand = new AnonymousCommand(OnAccept);
            CancelCommand = new AnonymousCommand(close);
        }

        public void Initialize()
        {
            ExportLocation.Initialize();
            ExportConditions.Initialize();
            ExportResolution.Initialize();
            ExportStrip.Initialize();
            ExportInformation.Initialize();
        }

        private void OnAccept()
        {
            if (ExportLocation.ResolvedPathErrorLevel == ExportLocationViewModel.ErrorLevel.Invalid)
                return;

            accept();
        }
    }
}
