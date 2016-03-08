using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using v4 = LayerDataReaderWriter.V4;
using v5 = LayerDataReaderWriter.V5;

namespace LayerDataReaderWriter.Conversion
{
    /// <summary>
    /// Represents a FormatConverter that transform data version 4 to version 5.
    /// </summary>
    public class V4ToV5FormatConverter : IFormatConverter
    {
        /// <summary>
        /// Always gets 4.
        /// </summary>
        public uint From
        {
            get { return 4; }
        }

        /// <summary>
        /// Always gets 5.
        /// </summary>
        public uint To
        {
            get { return 5; }
        }

        /// <summary>
        /// Converts data version 4 to version 5.
        /// </summary>
        /// <param name="inputData">Version 4 input data.</param>
        /// <returns>Returns version 5 data.</returns>
        public object Convert(object inputData)
        {
            return ((v4.LayerBlock[])inputData)
                .Select(block => new v5.LayerBlock(block.Identifier, block.Difficulty, block.Width, block.Height, ConvertUserFlags(block.Flags), new v5.SystemFlags(block.Flags[1]), block.Elements
                    .Select(elem => new v5.LayerBlockElement(elem.ID, elem.Tx, elem.Ty, 0.5f, 0.5f, elem.Angle, elem.Sx, elem.Sy, elem.Type, elem.Components
                        .Select(comp => new v5.Component(comp.Type, ConvertProperty(comp.Property)))
                        .ToArray()
                    ))
                    .ToArray()
                ))
                .ToArray();
        }

        private bool[] ConvertUserFlags(bool[] flags)
        {
            var result = new bool[8];
            result[0] = flags[0]; // is take off
            return result;
        }

        private v5.ComponentProperty ConvertProperty(v4.ComponentProperty prop)
        {
            if (prop == null)
                return null;

            switch (prop.Type)
            {
                case v4.ComponentPropertyType.Boolean: return new v5.ComponentProperty(prop.Name, prop.BooleanValue);
                case v4.ComponentPropertyType.Integer: return new v5.ComponentProperty(prop.Name, prop.IntegerValue);
                case v4.ComponentPropertyType.Float: return new v5.ComponentProperty(prop.Name, prop.FloatValue);
                case v4.ComponentPropertyType.String: return new v5.ComponentProperty(prop.Name, prop.StringValue);
                case v4.ComponentPropertyType.Guid: return new v5.ComponentProperty(prop.Name, prop.GuidValue);
            }
            throw new ArgumentException("Invalid component property");
        }
    }
}
