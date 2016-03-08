using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.UI.Core;
using LayerDataReaderWriter.V9;
using LevelEditor.Extensibility;

namespace LevelEditor.ViewModels.Validation
{
    public class ValidatorLogMessageViewModel : ViewModelBase
    {
        public LayerBlock Block { get; private set; }
        public SeverityLevel Severity { get; private set; }
        public string Message { get; private set; }

        public ValidatorLogMessageViewModel(SeverityLevel severity, string message, LayerBlock block, int elementIndex)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("Invalid 'message' argument.", "message");

            Block = block;

            Severity = severity;
            Message = message;
        }
    }
}
