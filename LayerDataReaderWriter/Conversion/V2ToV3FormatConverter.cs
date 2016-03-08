using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using v2 = LayerDataReaderWriter.V2;
using v3 = LayerDataReaderWriter.V3;

namespace LayerDataReaderWriter.Conversion
{
    /// <summary>
    /// Represents a FormatConverter that transform data version 2 to version 3.
    /// </summary>
    public class V2ToV3FormatConverter : IFormatConverter
    {
        /// <summary>
        /// Always gets 2.
        /// </summary>
        public uint From
        {
            get { return 2; }
        }

        /// <summary>
        /// Always gets 3.
        /// </summary>
        public uint To
        {
            get { return 3; }
        }

        /// <summary>
        /// Converts data version 2 to version 3.
        /// </summary>
        /// <param name="inputData">Version 2 input data.</param>
        /// <returns>Returns version 3 data.</returns>
        public object Convert(object inputData)
        {
            return ((v2.LayerBlock[])inputData)
                .Select(block => new v3.LayerBlock(block.Identifier, block.Difficulty, block.Width, block.Height, block.IsTakeOffBlock, block.IsEnabled, block.Elements
                    .Select(elem => new v3.LayerBlockElement(GetNextElementID(block), elem.Tx, elem.Ty, elem.Angle, elem.Sx, elem.Sy, (v3.LayerBlockElementType)elem.Type, elem.Components
                        .Select(comp => new v3.Component(comp.Type))
                        .ToArray()
                    ))
                    .ToArray()
                ))
                .ToArray();
        }

        private IDictionary<byte, byte> ids = new Dictionary<byte, byte>();

        private byte GetNextElementID(v2.LayerBlock block)
        {
            byte current;
            if (ids.TryGetValue(block.Identifier, out current) == false)
                current = 0;
            current++;
            ids[block.Identifier] = current;
            return current;
        }
    }
}
