using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter.V1
{
    /// <summary>
    /// Represent a Layer data reader version 1.
    /// </summary>
    public class ReaderV1 : IReader
    {
        /// <summary>
        /// Gets 1.
        /// </summary>
        public uint Version
        {
            get { return 1; }
        }

        private LayerBlockElement ReadLayerBlockElement(BinaryReader reader)
        {
            return new LayerBlockElement(
                reader.ReadSingle(), // tx
                reader.ReadSingle(), // ty
                reader.ReadSingle(), // angle
                reader.ReadSingle(), // sx
                reader.ReadSingle(), // sy
                (LayerBlockElementType)reader.ReadByte());
        }

        private LayerBlockElement[] ReadLayerBlockElements(ushort elementCount, BinaryReader reader)
        {
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

            ushort elementCount = reader.ReadUInt16();

            return new LayerBlock(identifier, difficulty, width, height, isTakeOffBlock, ReadLayerBlockElements(elementCount, reader));
        }

        private bool FlagToBoolean(byte allFlags, LayerBlockFlags testFlag)
        {
            return (allFlags & (1 << (int)LayerBlockFlags.TakeOffBlock)) != 0;
        }

        /// <summary>
        /// Reads Layer data version 1 from a BinaryReader.
        /// </summary>
        /// <param name="reader">The BinaryReader to read data from.</param>
        /// <returns>Returns a version 1 Layer data structure.</returns>
        public object Read(BinaryReader reader)
        {
            int blockCount = reader.ReadByte();

            var blocks = new LayerBlock[blockCount];
            for (int i = 0; i < blockCount; i++)
                blocks[i] = ReadLayerBlock(reader);

            return blocks;
        }
    }
}
