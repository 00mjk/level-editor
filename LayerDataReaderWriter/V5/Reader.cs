using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter.V5
{
    /// <summary>
    /// Represent a Layer data reader version 5.
    /// </summary>
    public class ReaderV5 : IReader
    {
        /// <summary>
        /// Gets 5.
        /// </summary>
        public uint Version
        {
            get { return 5; }
        }

        private Component ReadComponent(BinaryReader reader)
        {
            byte typeStringTableIndex = reader.ReadByte();
            var propertyType = reader.ReadByte();

            ComponentProperty property = null;

            if (propertyType > 0)
            {
                switch ((ComponentPropertyType)(propertyType - 1))
                {
                    case ComponentPropertyType.Boolean:
                        property = new ComponentProperty(null, reader.ReadBoolean());
                        break;
                    case ComponentPropertyType.Integer:
                        property = new ComponentProperty(null, reader.ReadInt32());
                        break;
                    case ComponentPropertyType.Float:
                        property = new ComponentProperty(null, reader.ReadSingle());
                        break;
                    case ComponentPropertyType.String:
                        property = new ComponentProperty(null, Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadUInt16())));
                        break;
                    case ComponentPropertyType.Guid:
                        property = new ComponentProperty(null, new Guid(reader.ReadBytes(16)));
                        break;
                    default:
                        //throw new InvalidDataException(string.Format("Unknown '{0}' property type.", propertyType));
                        throw new Exception(string.Format("Invalid property data: Unknown '{0}' property type.", propertyType));
                }
            }

            return new Component(stringTable[typeStringTableIndex], property);
        }

        private Component[] ReadComponents(BinaryReader reader)
        {
            int componentCount = reader.ReadByte();

            var components = new Component[componentCount];
            for (int i = 0; i < componentCount; i++)
                components[i] = ReadComponent(reader);

            return components;
        }

        private LayerBlockElement ReadLayerBlockElement(BinaryReader reader)
        {
            return new LayerBlockElement(
                reader.ReadUInt16(), // ID
                reader.ReadSingle(), // tx
                reader.ReadSingle(), // ty
                reader.ReadSingle(), // px
                reader.ReadSingle(), // py
                reader.ReadSingle(), // angle
                reader.ReadSingle(), // sx
                reader.ReadSingle(), // sy
                reader.ReadByte(), // type
                ReadComponents(reader));
        }

        private LayerBlockElement[] ReadLayerBlockElements(BinaryReader reader)
        {
            ushort elementCount = reader.ReadUInt16();

            var layerBlockElements = new LayerBlockElement[elementCount];

            for (uint i = 0; i < elementCount; i++)
                layerBlockElements[i] = ReadLayerBlockElement(reader);

            return layerBlockElements;
        }

        private LayerBlock ReadLayerBlock(BinaryReader reader)
        {
            byte identifier = reader.ReadByte();
            byte difficulty = reader.ReadByte();
            float width = reader.ReadSingle();
            float height = reader.ReadSingle();

            byte userFlags = reader.ReadByte();
            bool[] userFlagArray = Enumerable.Range(0, 8).Select(i => (userFlags & (1 << i)) != 0).ToArray();

            byte sysFlags = reader.ReadByte();
            bool[] sysFlagArray = Enumerable.Range(0, 8).Select(i => (sysFlags & (1 << i)) != 0).ToArray();

            var systemFlags = new SystemFlags(sysFlagArray[0]);

            var blockSize = reader.ReadUInt32();

            return new LayerBlock(identifier, difficulty, width, height, userFlagArray, systemFlags, ReadLayerBlockElements(reader));
        }

        private StringTable stringTable;

        /// <summary>
        /// Reads Layer data version 5 from a BinaryReader.
        /// </summary>
        /// <param name="reader">The BinaryReader to read data from.</param>
        /// <returns>Returns a version 5 Layer data structure.</returns>
        public object Read(BinaryReader reader)
        {
            stringTable = new StringTable(1);
            stringTable.Load(reader);

            int blockCount = reader.ReadByte();

            var blocks = new LayerBlock[blockCount];
            for (int i = 0; i < blockCount; i++)
                blocks[i] = ReadLayerBlock(reader);

            return blocks;
        }
    }
}
