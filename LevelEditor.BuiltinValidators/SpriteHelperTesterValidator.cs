using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LayerDataReaderWriter.V9;
using LevelEditor.Extensibility;

namespace LevelEditor.BuiltinValidators
{
    [Export(typeof(IValidator))]
    [ExportMetadata("Name", "SpriteHelper Tester Validator")]
    [ExportMetadata("Description", "Validator that unit tests the default SpriteHelper implementation.")]
    public class SpriteHelperTesterValidator : IValidator
    {
        public void Validate(IValidationController controller, LayerBlock[] blocks, IGeometryHelper spriteHelper)
        {
            if (blocks.Length == 0)
            {
                controller.Report(new ValidationEntry(SeverityLevel.Fatal, "It must have at least one block", null));
                return;
            }

            var block = blocks[0];

            if (block.Elements == null || block.Elements.Length < 2)
            {
                controller.Report(new ValidationEntry(SeverityLevel.Fatal, "The first block must have at least two elements", block));
                return;
            }

            var passed = true;
            var elem1 = block.Elements[0];
            var elem2 = block.Elements[1];

            var rect = spriteHelper.GetAxisAlignedBoundingBox(block, elem1);
            if (passed &= rect.Contains(elem2.Tx, elem2.Ty) == false)
                controller.Report(new ValidationEntry(SeverityLevel.Info, "Center position of element 2 is not inside transformed bounding box of element 1", block));

            if (passed &= spriteHelper.AreColliding(block, elem1, elem2) == false)
                controller.Report(new ValidationEntry(SeverityLevel.Info, "Element 1 and element 2 are not colliding", block));

            if (passed &= spriteHelper.AreColliding(block, elem1, new Point(0.0, 0.0), new Point(1.0, 1.0)) == false)
                controller.Report(new ValidationEntry(SeverityLevel.Info, "Element 1 does not interset line from 0,0 to 1,1", block));

            if (passed &= spriteHelper.AreColliding(block, elem2, new Point(0.0, 1.0), new Point(1.0, 0.0)) == false)
                controller.Report(new ValidationEntry(SeverityLevel.Info, "Element 2 does not interset line from 0,1 to 1,0", block));

            if(passed == false)
                controller.Report(new ValidationEntry(SeverityLevel.Error, "The sprite helper validator did not pass all the tests", block));
        }
    }
}
