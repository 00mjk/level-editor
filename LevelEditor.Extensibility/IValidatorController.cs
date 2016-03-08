using System;

namespace LevelEditor.Extensibility
{
    public interface IValidationController
    {
        void Report(ValidationEntry entry);
    }
}
