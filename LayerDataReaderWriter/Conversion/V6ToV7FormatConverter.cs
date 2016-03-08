using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using v6 = LayerDataReaderWriter.V6;
using v7 = LayerDataReaderWriter.V7;

namespace LayerDataReaderWriter.Conversion
{
    /// <summary>
    /// Represents a FormatConverter that transform data version 6 to version 7.
    /// </summary>
    public class V6ToV7FormatConverter : IFormatConverter
    {
        /// <summary>
        /// Always gets 6.
        /// </summary>
        public uint From
        {
            get { return 6; }
        }

        /// <summary>
        /// Always gets 7.
        /// </summary>
        public uint To
        {
            get { return 7; }
        }

        /// <summary>
        /// Converts data version 6 to version 7.
        /// </summary>
        /// <param name="inputData">Version 6 input data.</param>
        /// <returns>Returns version 7 data.</returns>
        public object Convert(object inputData)
        {
            var layerBlocks = ((v6.LayerBlock[])inputData)
                .Select(block => new v7.LayerBlock(block.Identifier, block.Difficulty, block.Width, block.Height, block.UserFlags, new v7.SystemFlags(block.SystemFlags.IsEnabled), block.Elements
                    .Select(elem => new v7.LayerBlockElement(elem.ID, elem.Tx, elem.Ty, elem.Px, elem.Py, elem.Angle, elem.Sx, elem.Sy, elem.Type, elem.Components
                        .Select(comp => new v7.Component(comp.Type, comp.Properties
                            .Select(ConvertProperty)
                            .ToArray()
                        ))
                        .ToArray()
                    ))
                    .ToArray()
                ))
                .ToArray();

            return new v7.LayerFile(null, layerBlocks);
        }

        private v7.ComponentProperty ConvertProperty(v6.ComponentProperty prop)
        {
            if (prop == null)
                return null;

            switch (prop.Type)
            {
                case v6.ComponentPropertyType.Boolean: return new v7.ComponentProperty(prop.Name, prop.BooleanValue);
                case v6.ComponentPropertyType.Integer: return new v7.ComponentProperty(prop.Name, prop.IntegerValue);
                case v6.ComponentPropertyType.Float: return new v7.ComponentProperty(prop.Name, prop.FloatValue);
                case v6.ComponentPropertyType.String: return new v7.ComponentProperty(prop.Name, prop.StringValue);
                case v6.ComponentPropertyType.Guid: return new v7.ComponentProperty(prop.Name, prop.GuidValue);
            }
            throw new ArgumentException("Invalid component property");
        }
    }
}
