using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter
{
    /// <summary>
    /// Represent a table of unique indexed strings.
    /// </summary>
    public class StringTable
    {
        private int headerBytesSize;
        private string[] strings;

        /// <summary>
        /// Gets the number of unique strings in to the table.
        /// </summary>
        public uint Count { get; private set; }

        /// <summary>
        /// A container that turns strings into indicies, and the other way around.
        /// </summary>
        /// <param name="headerBytesSize">The number of bytes used to store the header.
        /// This determines the string table maximum capacity.</param>
        public StringTable(int headerBytesSize)
        {
            if ((headerBytesSize == 1 || headerBytesSize == 2 || headerBytesSize == 4) == false)
                throw new ArgumentOutOfRangeException("headerBytesSize", "Invalid 'headerBytesSize' argument. It must be 1, 2, or 4.");

            this.headerBytesSize = headerBytesSize;
            strings = new string[headerBytesSize * 256];
        }

        /// <summary>
        /// Add a string to the table and get its index.
        /// If the given string is already in the table, it returns its index anyway.
        /// It throws an OverflowException exception if the maximum capacity of the table is reached.
        /// </summary>
        /// <param name="str">The string to add to the table.</param>
        /// <returns>Returns the index of the string, whether it already exists in the table or not.</returns>
        public int Add(string str)
        {
            str = str ?? string.Empty;

            int i = 0;

            while (i < strings.Length && strings[i] != null)
            {
                if (string.Equals(strings[i], str, StringComparison.Ordinal))
                    return i;
                i++;
            }

            if (i >= strings.Length)
                throw new OverflowException("Maximum string table capacity reached.");

            strings[i] = str;
            Count++;

            return i;
        }

        /// <summary>
        /// Gets the index of the given string in the table.
        /// </summary>
        /// <param name="str">The string to look for index.</param>
        /// <returns>Returns the index of the string if it exists, -1 otherwise.</returns>
        public int IndexOf(string str)
        {
            str = str ?? string.Empty;

            int i = 0;

            while (i < Count && strings[i] != null)
            {
                if (string.Equals(strings[i], str, StringComparison.Ordinal))
                    return i;
                i++;
            }

            return -1;
        }

        /// <summary>
        /// Gets the string corresponding to the index.
        /// It throws an ArgumentOutOfRangeException exception if index is beyound limits.
        /// </summary>
        /// <param name="index">Index of the string to get.</param>
        /// <returns>Returns the string located at the index in the table.</returns>
        public string this[int index]
        {
            get
            {
                return strings[index];
            }
        }

        /// <summary>
        /// Clears the whole string table.
        /// </summary>
        public void Clear()
        {
            Array.Clear(strings, 0, strings.Length);
            Count = 0;
        }

        /// <summary>
        /// Loads the string table from binary data.
        /// It clears the table first and then load data, so previously added strings are lost when loading.
        /// </summary>
        /// <param name="reader">The BinaryReader instance used to read data.</param>
        public void Load(BinaryReader reader)
        {
            uint entryCount;

            if (headerBytesSize == 1)
               entryCount = reader.ReadByte();
            else if (headerBytesSize == 2)
                entryCount = reader.ReadUInt16();
            else
                entryCount = reader.ReadUInt32();

            Clear();

            for (int i = 0; i < entryCount; i++)
                Add(reader.ReadString());
        }

        /// <summary>
        /// Writes the string table to binary data.
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            if (headerBytesSize == 1)
                writer.Write((byte)Count);
            else if (headerBytesSize == 2)
                writer.Write((ushort)Count);
            else
                writer.Write((uint)Count);

            for (int i = 0; i < Count; i++)
                writer.Write(strings[i]);
        }
    }
}
