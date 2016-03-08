using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter.V1
{
    /// <summary>
    /// Represent a Layer data writer version 1.
    /// </summary>
    public class WriterV1 : IWriter
    {
        /// <summary>
        /// Gets 1.
        /// </summary>
        public uint Version
        {
            get { return 1; }
        }

        private void WriteLayerBlockElement(LayerBlockElement element, BinaryWriter writer)
        {
            writer.Write(element.Tx);
            writer.Write(element.Ty);
            writer.Write(element.Angle);
            writer.Write(element.Sx);
            writer.Write(element.Sy);
            writer.Write((byte)element.Type);
        }

        private void WriteLayerBlockElements(LayerBlockElement[] elements, BinaryWriter writer)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");

            writer.Write((ushort)elements.Length);
            foreach (var element in elements)
                WriteLayerBlockElement(element, writer);
        }

        private void WriteLayerBlock(LayerBlock layerBlock, BinaryWriter writer)
        {
            if (layerBlock == null)
                throw new ArgumentNullException("layerBlock");

            writer.Write(layerBlock.Identifier);
            writer.Write(layerBlock.Difficulty);
            writer.Write(layerBlock.Width);
            writer.Write(layerBlock.Height);

            byte flags = 0;
            UpdateFlag(ref flags, layerBlock.IsTakeOffBlock, LayerBlockFlags.TakeOffBlock);

            writer.Write(flags);

            WriteLayerBlockElements(layerBlock.Elements, writer);
        }

        private void UpdateFlag(ref byte allFlags, bool isSet, LayerBlockFlags newFlag)
        {
            if (isSet)
                allFlags |= (byte)(1 << (int)newFlag);
        }

        /// <summary>
        /// Writes Layer data version 1 to a BinaryWriter.
        /// </summary>
        /// <param name="data">The version 1 data structure to write.</param>
        /// <param name="writer">The BinaryWriter to write data to.</param>
        public void Write(object data, BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            var layerBlocks = data as LayerBlock[];
 
            if (layerBlocks == null)
                throw new ArgumentNullException("layerBlocks");

            writer.Write((byte)layerBlocks.Length);

            foreach (var block in layerBlocks)
                WriteLayerBlock(block, writer);
        }
    }
}
