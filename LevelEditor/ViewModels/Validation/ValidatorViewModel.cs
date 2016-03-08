using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Bitcraft.Core.Logging;
using Bitcraft.UI.Core;
using LayerDataReaderWriter.V9;
using LevelEditor.Core;
using LevelEditor.Extensibility;

namespace LevelEditor.ViewModels.Validation
{
    public class ValidatorViewModel : RootedViewModel
    {
        private bool? status;
        public bool? Status
        {
            get { return status; }
            private set { SetValue(ref status, value); }
        }

        public string Name { get; private set; }
        public string Description { get; private set; }

        private Dispatcher dispatcher;
        private ValidationController controller;

        private Lazy<IValidator, IValidatorMetadata> validatorContainer;

        private ObservableCollection<ValidatorLogMessageViewModel> logMessages = new ObservableCollection<ValidatorLogMessageViewModel>();
        public System.Collections.ObjectModel.ReadOnlyObservableCollection<ValidatorLogMessageViewModel> LogMessages { get; private set; }

        public ValidatorLogMessageViewModel SelectedLogMessage { get; set; }

        public ValidatorViewModel(RootViewModel root, Lazy<IValidator, IValidatorMetadata> validatorContainer)
            : base(root)
        {
            if (validatorContainer == null)
                throw new ArgumentNullException("validatorContainer");
            this.validatorContainer = validatorContainer;

            dispatcher = Dispatcher.CurrentDispatcher;

            LogMessages = new System.Collections.ObjectModel.ReadOnlyObservableCollection<ValidatorLogMessageViewModel>(logMessages);
            controller = new ValidationController(dispatcher, logMessages.Add);

            Name = validatorContainer.Metadata.Name;
            Description = validatorContainer.Metadata.Description;

            RunCommand = new AnonymousCommand(OnRun);
            InvestigateCommand = new AnonymousCommand(OnInvestigate);
        }

        public ICommand RunCommand { get; private set; }
        public ICommand InvestigateCommand { get; private set; }

        private void OnRun()
        {
            ThreadPool.QueueUserWorkItem(_ =>
                {
                    RunSynchronously(Root.LayerData.Blocks.Select(x => x.ProduceLayerBlock()).ToArray());
                });
        }

        private void OnInvestigate()
        {
            if (SelectedLogMessage == null || SelectedLogMessage.Block == null)
                return;

            var blockId = SelectedLogMessage.Block.Identifier;

            var relatedBlock = Root.LayerData.Blocks.FirstOrDefault(b => b.Identifier == blockId);
            if (relatedBlock != null)
                Root.LayerData.SelectedBlock = relatedBlock;
        }

        public void ClearLog()
        {
            dispatcher.BeginInvoke((Action)delegate() { logMessages.Clear(); });
        }

        public void RunSynchronously(LayerBlock[] blocks)
        {
            ClearLog();
            Status = null;
            controller.Reset();
            validatorContainer.Value.Validate(controller, blocks, new GeometryHelper());
            Status = controller.Status;
        }
    }
}
