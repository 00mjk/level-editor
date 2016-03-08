using System;
using LayerDataReaderWriter.V9;

namespace LevelEditor.Extensibility
{
    public enum SeverityLevel
    {
        Info,
        Warning,
        Error,
        Fatal,
    }

    public class ValidationEntry
    {
        public ValidationEntry(SeverityLevel severity, string message, LayerBlock block)
            : this(severity, message, block, -1)
        {
        }

        public ValidationEntry(SeverityLevel severity, string message, LayerBlock block, int elementIndex)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Invalid 'message' argument.", "message");

            Severity = severity;
            Message = message;
            Block = block;
            ElementIndex = elementIndex;
        }

        public SeverityLevel Severity { get; private set; }
        public string Message { get; private set; }
        public LayerBlock Block { get; private set; }
        public int ElementIndex { get; private set; }
    }
}
