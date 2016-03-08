using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using LevelEditor.Extensibility;

namespace LevelEditor.ViewModels.Validation
{
    public class ValidationController : IValidationController
    {
        private Action<ValidatorLogMessageViewModel> log;
        private Dispatcher dispatcher;

        public bool Status { get; private set; }

        public ValidationController(Dispatcher dispatcher, Action<ValidatorLogMessageViewModel> log)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");
            if (log == null)
                throw new ArgumentNullException("log");

            this.dispatcher = dispatcher;
            this.log = log;

            Reset();
        }

        public void Reset()
        {
            Status = true;
        }

        public void Report(ValidationEntry entry)
        {
            dispatcher.BeginInvoke((Action)delegate()
            {
                log(new ValidatorLogMessageViewModel(entry.Severity, entry.Message, entry.Block, entry.ElementIndex));
            });

            if (entry.Severity == SeverityLevel.Fatal || entry.Severity == SeverityLevel.Error)
                Status = false;
        }
    }
}
