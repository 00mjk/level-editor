using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using v3 = LayerDataReaderWriter.V3;
using v4 = LayerDataReaderWriter.V4;

namespace LayerDataReaderWriter.Conversion
{
    /// <summary>
    /// Represents a FormatConverter that transform data version 3 to version 4.
    /// </summary>
    public class V3ToV4FormatConverter : IFormatConverter
    {
        /// <summary>
        /// Always gets 3.
        /// </summary>
        public uint From
        {
            get { return 3; }
        }

        /// <summary>
        /// Always gets 4.
        /// </summary>
        public uint To
        {
            get { return 4; }
        }

        /// <summary>
        /// Converts data version 3 to version 4.
        /// </summary>
        /// <param name="inputData">Version 3 input data.</param>
        /// <returns>Returns version 4 data.</returns>
        public object Convert(object inputData)
        {
            return ((v3.LayerBlock[])inputData)
                .Select(block => new v4.LayerBlock(block.Identifier, block.Difficulty, block.Width, block.Height, ConvertFlags(block), block.Elements
                    .Select(elem => new v4.LayerBlockElement(elem.ID, elem.Tx, elem.Ty, elem.Angle, elem.Sx, elem.Sy, (byte)elem.Type, elem.Components
                        .Select(comp => new v4.Component(comp.Type, ConvertProperty(comp.Property)))
                        .ToArray()
                    ))
                    .ToArray()
                ))
                .ToArray();
        }

        private bool[] ConvertFlags(v3.LayerBlock block)
        {
            var result = new bool[8];
            result[0] = block.IsTakeOffBlock;
            result[1] = block.IsEnabled;
            return result;
        }

        private v4.ComponentProperty ConvertProperty(v3.ComponentProperty prop)
        {
            if (prop == null)
                return null;

            switch (prop.Type)
            {
                case v3.ComponentPropertyType.Boolean: return new v4.ComponentProperty(prop.Name, prop.BooleanValue);
                case v3.ComponentPropertyType.Integer: return new v4.ComponentProperty(prop.Name, prop.IntegerValue);
                case v3.ComponentPropertyType.Float: return new v4.ComponentProperty(prop.Name, prop.FloatValue);
                case v3.ComponentPropertyType.String: return new v4.ComponentProperty(prop.Name, prop.StringValue);
                case v3.ComponentPropertyType.Guid: return new v4.ComponentProperty(prop.Name, prop.GuidValue);
            }
            throw new ArgumentException("Invalid component property");
        }
    }
}
