using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter.V3
{
    /// <summary>
    /// Represent a Layer data writer version 3.
    /// </summary>
    public class WriterV3 : IWriter
    {
        /// <summary>
        /// Gets 3.
        /// </summary>
        public uint Version
        {
            get { return 3; }
        }

        private void WriteProperty(BinaryWriter bw, ComponentProperty property)
        {
            if (property == null)
            {
                bw.Write((byte)0);
                return;
            }

            bw.Write((byte)(property.Type + 1));

            switch (property.Type)
            {
                case ComponentPropertyType.Boolean:
                    bw.Write(property.BooleanValue);
                    break;
                case ComponentPropertyType.Integer:
                    bw.Write(property.IntegerValue);
                    break;
                case ComponentPropertyType.Float:
                    bw.Write(property.FloatValue);
                    break;
                case ComponentPropertyType.String:
                    var data = Encoding.UTF8.GetBytes(property.StringValue);
                    bw.Write((ushort)data.Length);
                    bw.Write(data);
                    break;
                case ComponentPropertyType.Guid:
                    bw.Write(property.GuidValue.ToByteArray());
                    break;
                default:
                    throw new InvalidDataException(string.Format("Unknown '{0}' property type.", property.Type));
            }
        }

        private void WriteComponent(BinaryWriter bw, Component component)
        {
            if (component == null)
                return;

            int index = stringTable.IndexOf(component.Type);
            if (index == -1)
                throw new ArgumentException(string.Format("Cannot find string '{0}' in string table.", component.Type));
            bw.Write((byte)index);

            WriteProperty(bw, component.Property);
        }

        private void WriteComponents(BinaryWriter bw, Component[] components)
        {
            bw.Write((byte)components.Length);

            foreach (var component in components)
                WriteComponent(bw, component);
        }

        private void WriteLayerBlockElement(BinaryWriter bw, LayerBlockElement element)
        {
            bw.Write(element.ID);
            bw.Write(element.Tx);
            bw.Write(element.Ty);
            bw.Write(element.Angle);
            bw.Write(element.Sx);
            bw.Write(element.Sy);
            bw.Write((byte)element.Type);
            WriteComponents(bw, element.Components ?? new Component[0]);
        }

        private void WriteLayerBlockElements(LayerBlockElement[] elements, BinaryWriter writer)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");

            byte[] blockData;

            using (var ms = new MemoryStream())
            {
                using (var tempWriter = new BinaryWriter(ms))
                {
                    tempWriter.Write((ushort)elements.Length);

                    foreach (var element in elements)
                        WriteLayerBlockElement(tempWriter, element);

                    blockData = ms.ToArray();
                }
            }

            writer.Write((uint)blockData.Length);
            writer.Write(blockData);
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
            UpdateFlag(ref flags, layerBlock.IsEnabled, LayerBlockFlags.EnabledBlock);

            writer.Write(flags);

            WriteLayerBlockElements(layerBlock.Elements, writer);
        }

        private void UpdateFlag(ref byte allFlags, bool isSet, LayerBlockFlags newFlag)
        {
            if (isSet)
                allFlags |= (byte)(1 << (int)newFlag);
        }

        private void CreateStringTable(LayerBlock[] blocks, BinaryWriter writer)
        {
            stringTable = new StringTable(1);

            foreach (var block in blocks)
            {
                foreach (var element in block.Elements)
                {
                    if (element.Components != null)
                    {
                        foreach (var component in element.Components)
                            stringTable.Add(component.Type);
                    }
                }
            }

            stringTable.Write(writer);
        }

        private StringTable stringTable;

        /// <summary>
        /// Writes Layer data version 3 to a BinaryWriter.
        /// </summary>
        /// <param name="data">The version 3 data structure to write.</param>
        /// <param name="writer">The BinaryWriter to write data to.</param>
        public void Write(object data, BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            var layerBlocks = data as LayerBlock[];

            if (layerBlocks == null)
                throw new ArgumentNullException("layerBlocks");

            CreateStringTable(layerBlocks, writer);

            writer.Write((byte)layerBlocks.Length);

            foreach (var block in layerBlocks)
                WriteLayerBlock(block, writer);
        }
    }
}
