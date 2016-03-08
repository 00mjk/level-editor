using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter.V7
{
    /// <summary>
    /// Represent a Layer data reader version 7.
    /// </summary>
    public class ReaderV7 : IReader
    {
        /// <summary>
        /// Gets 7.
        /// </summary>
        public uint Version
        {
            get { return 7; }
        }

        private ComponentProperty ReadComponentProperty(BinaryReader reader)
        {
            ushort propStringTableIndex = reader.ReadUInt16();
            var propName = stringTable[propStringTableIndex];

            var propertyType = reader.ReadByte();

            ComponentProperty property = null;

            switch ((ComponentPropertyType)propertyType)
            {
                case ComponentPropertyType.Boolean:
                    property = new ComponentProperty(propName, reader.ReadBoolean());
                    break;
                case ComponentPropertyType.Integer:
                    property = new ComponentProperty(propName, reader.ReadInt32());
                    break;
                case ComponentPropertyType.Float:
                    property = new ComponentProperty(propName, reader.ReadSingle());
                    break;
                case ComponentPropertyType.String:
                    property = new ComponentProperty(propName, Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadUInt16())));
                    break;
                case ComponentPropertyType.Guid:
                    property = new ComponentProperty(propName, new Guid(reader.ReadBytes(16)));
                    break;
                default:
                    throw new Exception(string.Format("Invalid property data: Unknown '{0}' property type.", propertyType));
            }

            return property;
        }

        private Component ReadComponent(BinaryReader reader)
        {
            ushort typeStringTableIndex = reader.ReadUInt16();
            byte propertiesCount = reader.ReadByte();

            var properties = new ComponentProperty[propertiesCount];

            for (int i = 0; i < propertiesCount; i++)
                properties[i] = ReadComponentProperty(reader);

            return new Component(stringTable[typeStringTableIndex], properties);
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

            bool[] userFlagArray = ReadFlags(reader);

            byte sysFlags = reader.ReadByte();
            bool[] sysFlagArray = Enumerable.Range(0, 8).Select(i => (sysFlags & (1 << i)) != 0).ToArray();

            var systemFlags = new SystemFlags(sysFlagArray[0]);

            var blockSize = reader.ReadUInt32();

            return new LayerBlock(identifier, difficulty, width, height, userFlagArray, systemFlags, ReadLayerBlockElements(reader));
        }

        private bool[] ReadFlags(BinaryReader reader)
        {
            int userFlagCount = reader.ReadByte();

            int remaining;
            int bytesToRead = Math.DivRem(userFlagCount, 8, out remaining);
            if (remaining > 0)
                bytesToRead++;

            return Enumerable.Range(0, bytesToRead)
                .Select(i => reader.ReadByte())
                .SelectMany(b => Enumerable.Range(0, 8).Select(i => (b & (1 << i)) != 0))
                .Take(userFlagCount)
                .ToArray();
        }

        private StringTable stringTable;

        /// <summary>
        /// Reads Layer data version 7 from a BinaryReader.
        /// </summary>
        /// <param name="reader">The BinaryReader to read data from.</param>
        /// <returns>Returns a version 7 LayerFile data structure.</returns>
        public object Read(BinaryReader reader)
        {
            stringTable = new StringTable(2);
            stringTable.Load(reader);

            var userFlags = ReadFlags(reader);

            int blockCount = reader.ReadByte();

            var blocks = new LayerBlock[blockCount];
            for (int i = 0; i < blockCount; i++)
                blocks[i] = ReadLayerBlock(reader);

            return new LayerFile(userFlags, blocks);
        }
    }
}
