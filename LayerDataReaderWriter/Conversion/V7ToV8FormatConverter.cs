using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using v7 = LayerDataReaderWriter.V7;
using v8 = LayerDataReaderWriter.V8;

namespace LayerDataReaderWriter.Conversion
{
    /// <summary>
    /// Represents a FormatConverter that transform data version 7 to version 8.
    /// </summary>
    public class V7ToV8FormatConverter : IFormatConverter
    {
        /// <summary>
        /// Always gets 7.
        /// </summary>
        public uint From
        {
            get { return 7; }
        }

        /// <summary>
        /// Always gets 8.
        /// </summary>
        public uint To
        {
            get { return 8; }
        }

        /// <summary>
        /// Converts data version 7 to version 8.
        /// </summary>
        /// <param name="inputData">Version 7 input data.</param>
        /// <returns>Returns version 8 data.</returns>
        public object Convert(object inputData)
        {
            var file = (v7.LayerFile)inputData;

            var layerBlocks = file.Blocks
                .Select(block => new v8.LayerBlock(block.Identifier, block.Difficulty, block.Width, block.Height, block.UserFlags, new v8.SystemFlags(block.SystemFlags.IsEnabled), block.Elements
                    .Select(elem => new v8.LayerBlockElement(elem.ID, elem.Tx, elem.Ty, elem.Px, elem.Py, elem.Angle, elem.Sx, elem.Sy, elem.Type, v8.ColliderType.Square, elem.Components
                        .Select(comp => new v8.Component(comp.Type, comp.Properties
                            .Select(ConvertProperty)
                            .ToArray()
                        ))
                        .ToArray()
                    ))
                    .ToArray()
                ))
                .ToArray();

            return new v8.LayerFile(file.UserFlags, layerBlocks);
        }

        private v8.ComponentProperty ConvertProperty(v7.ComponentProperty prop)
        {
            if (prop == null)
                return null;

            switch (prop.Type)
            {
                case v7.ComponentPropertyType.Boolean: return new v8.ComponentProperty(prop.Name, prop.BooleanValue);
                case v7.ComponentPropertyType.Integer: return new v8.ComponentProperty(prop.Name, prop.IntegerValue);
                case v7.ComponentPropertyType.Float: return new v8.ComponentProperty(prop.Name, prop.FloatValue);
                case v7.ComponentPropertyType.String: return new v8.ComponentProperty(prop.Name, prop.StringValue);
                case v7.ComponentPropertyType.Guid: return new v8.ComponentProperty(prop.Name, prop.GuidValue);
            }
            throw new ArgumentException("Invalid component property");
        }
    }
}
