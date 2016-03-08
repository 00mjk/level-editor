using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter.V2
{
    /// <summary>
    /// Represent a Layer data reader version 2.
    /// </summary>
    public class ReaderV2 : IReader
    {
        /// <summary>
        /// Gets 2.
        /// </summary>
        public uint Version
        {
            get { return 2; }
        }

        private Component[] ReadComponents(BinaryReader reader)
        {
            int componentCount = reader.ReadByte();

            var components = new Component[componentCount];
            for (int i = 0; i < componentCount; i++)
            {
                byte index = reader.ReadByte();
                components[i] = new Component(stringTable[index]);
            }

            return components;
        }

        private LayerBlockElement ReadLayerBlockElement(BinaryReader reader)
        {
            return new LayerBlockElement(
                reader.ReadSingle(), // tx
                reader.ReadSingle(), // ty
                reader.ReadSingle(), // angle
                reader.ReadSingle(), // sx
                reader.ReadSingle(), // sy
                (LayerBlockElementType)reader.ReadByte(),
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

            byte flags = reader.ReadByte();
            bool isTakeOffBlock = FlagToBoolean(flags, LayerBlockFlags.TakeOffBlock);
            bool isEnabled = FlagToBoolean(flags, LayerBlockFlags.EnabledBlock);

            var blockSize = reader.ReadUInt32();

            return new LayerBlock(identifier, difficulty, width, height, isTakeOffBlock, isEnabled, ReadLayerBlockElements(reader));
        }

        private bool FlagToBoolean(byte allFlags, LayerBlockFlags testFlag)
        {
            return (allFlags & (1 << (int)testFlag)) != 0;
        }

        private StringTable stringTable;

        /// <summary>
        /// Reads Layer data version 2 from a BinaryReader.
        /// </summary>
        /// <param name="reader">The BinaryReader to read data from.</param>
        /// <returns>Returns a version 2 Layer data structure.</returns>
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
