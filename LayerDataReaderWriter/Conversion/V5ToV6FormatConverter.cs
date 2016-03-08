using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using v5 = LayerDataReaderWriter.V5;
using v6 = LayerDataReaderWriter.V6;

namespace LayerDataReaderWriter.Conversion
{
    /// <summary>
    /// Represents a FormatConverter that transform data version 5 to version 6.
    /// </summary>
    public class V5ToV6FormatConverter : IFormatConverter
    {
        /// <summary>
        /// Always gets 5.
        /// </summary>
        public uint From
        {
            get { return 5; }
        }

        /// <summary>
        /// Always gets 6.
        /// </summary>
        public uint To
        {
            get { return 6; }
        }

        /// <summary>
        /// Converts data version 5 to version 6.
        /// </summary>
        /// <param name="inputData">Version 5 input data.</param>
        /// <returns>Returns version 6 data.</returns>
        public object Convert(object inputData)
        {
            return ((v5.LayerBlock[])inputData)
                .Select(block => new v6.LayerBlock(block.Identifier, block.Difficulty, block.Width, block.Height, block.UserFlags, new v6.SystemFlags(block.SystemFlags.IsEnabled), block.Elements
                    .Select(elem => new v6.LayerBlockElement(elem.ID, elem.Tx, elem.Ty, elem.Px, elem.Py, elem.Angle, elem.Sx, elem.Sy, elem.Type, elem.Components
                        .Select(comp => new v6.Component(comp.Type, ConvertProperty(comp.Property)))
                        .ToArray()
                    ))
                    .ToArray()
                ))
                .ToArray();
        }

        private v6.ComponentProperty ConvertProperty(v5.ComponentProperty prop)
        {
            if (prop == null)
                return null;

            switch (prop.Type)
            {
                case v5.ComponentPropertyType.Boolean: return new v6.ComponentProperty(prop.Name, prop.BooleanValue);
                case v5.ComponentPropertyType.Integer: return new v6.ComponentProperty(prop.Name, prop.IntegerValue);
                case v5.ComponentPropertyType.Float: return new v6.ComponentProperty(prop.Name, prop.FloatValue);
                case v5.ComponentPropertyType.String: return new v6.ComponentProperty(prop.Name, prop.StringValue);
                case v5.ComponentPropertyType.Guid: return new v6.ComponentProperty(prop.Name, prop.GuidValue);
            }
            throw new ArgumentException("Invalid component property");
        }
    }
}
