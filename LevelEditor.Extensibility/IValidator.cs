using System;
using LayerDataReaderWriter.V9;

namespace LevelEditor.Extensibility
{
    public interface IValidator
    {
        void Validate(IValidationController controller, LayerBlock[] blocks, IGeometryHelper spriteHelper);
    }
}
