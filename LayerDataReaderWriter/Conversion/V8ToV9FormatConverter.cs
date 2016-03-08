using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using v8 = LayerDataReaderWriter.V8;
using v9 = LayerDataReaderWriter.V9;

namespace LayerDataReaderWriter.Conversion
{
    /// <summary>
    /// Represents a FormatConverter that transform data version 8 to version 9.
    /// </summary>
    public class V8ToV9FormatConverter : IFormatConverter
    {
        /// <summary>
        /// Always gets 8.
        /// </summary>
        public uint From
        {
            get { return 8; }
        }

        /// <summary>
        /// Always gets 9.
        /// </summary>
        public uint To
        {
            get { return 9; }
        }

        /// <summary>
        /// Converts data version 8 to version 9.
        /// </summary>
        /// <param name="inputData">Version 8 input data.</param>
        /// <returns>Returns version 9 data.</returns>
        public object Convert(object inputData)
        {
            var file = (v8.LayerFile)inputData;

            var layerBlocks = file.Blocks
                .Select(block => new v9.LayerBlock(block.Identifier, block.Difficulty, block.Width, block.Height, block.UserFlags, new v9.SystemFlags(block.SystemFlags.IsEnabled), block.Elements
                    .Select(elem => new v9.LayerBlockElement(elem.ID, elem.Tx, elem.Ty, 0, 0, elem.Px, elem.Py, elem.Angle, elem.Sx, elem.Sy, elem.Type, v9.ColliderType.Square, elem.Components
                        .Select(comp => new v9.Component(comp.Type, comp.Properties
                            .Select(ConvertProperty)
                            .ToArray()
                        ))
                        .ToArray()
                    ))
                    .ToArray()
                ))
                .ToArray();

            return new v9.LayerFile(file.UserFlags, layerBlocks);
        }

        private v9.ComponentProperty ConvertProperty(v8.ComponentProperty prop)
        {
            if (prop == null)
                return null;

            switch (prop.Type)
            {
                case v8.ComponentPropertyType.Boolean: return new v9.ComponentProperty(prop.Name, prop.BooleanValue);
                case v8.ComponentPropertyType.Integer: return new v9.ComponentProperty(prop.Name, prop.IntegerValue);
                case v8.ComponentPropertyType.Float: return new v9.ComponentProperty(prop.Name, prop.FloatValue);
                case v8.ComponentPropertyType.String: return new v9.ComponentProperty(prop.Name, prop.StringValue);
                case v8.ComponentPropertyType.Guid: return new v9.ComponentProperty(prop.Name, prop.GuidValue);
            }
            throw new ArgumentException("Invalid component property");
        }
    }
}
