using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LayerDataReaderWriter
{
    /// <summary>
    /// Represents a BinaryReader that is endianness aware.
    /// </summary>
    public class CustomBinaryReader : BinaryReader
    {
        /// <summary>
        /// Tells whether to swap the endianness when reading data.
        /// </summary>
        public bool ChangeEndianness { get; set; }

        /// <summary>
        /// Initializes the CustomBinaryReader instance.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="encoding">The encoding to use where needed.</param>
        public CustomBinaryReader(Stream stream, Encoding encoding)
            : base(stream, encoding ?? Encoding.UTF8)
        {
        }

        /// <summary>
        /// Reads a double precision floating point value.
        /// </summary>
        /// <returns>Returns a double precision floating point value.</returns>
        public override double ReadDouble()
        {
            if (ChangeEndianness == false)
                return base.ReadDouble();
            return ReadAndSwap<double>(BitConverter.ToDouble);
        }

        /// <summary>
        /// Reads a 16 bits signed integer value.
        /// </summary>
        /// <returns>Returns a 16 bits signed integer value.</returns>
        public override short ReadInt16()
        {
            if (ChangeEndianness == false)
                return base.ReadInt16();
            return ReadAndSwap<short>(BitConverter.ToInt16);
        }

        /// <summary>
        /// Reads a 32 bits signed integer value.
        /// </summary>
        /// <returns>Returns a 32 bits signed integer value.</returns>
        public override int ReadInt32()
        {
            if (ChangeEndianness == false)
                return base.ReadInt32();
            return ReadAndSwap<int>(BitConverter.ToInt32);
        }

        /// <summary>
        /// Reads a 64 bits signed integer value.
        /// </summary>
        /// <returns>Returns a 64 bits signed integer value.</returns>
        public override long ReadInt64()
        {
            if (ChangeEndianness == false)
                return base.ReadInt64();
            return ReadAndSwap<long>(BitConverter.ToInt64);
        }

        /// <summary>
        /// Reads a single precision floating point value.
        /// </summary>
        /// <returns>Returns a single precision floating point value.</returns>
        public override float ReadSingle()
        {
            if (ChangeEndianness == false)
                return base.ReadSingle();
            return ReadAndSwap<float>(BitConverter.ToSingle);
        }

        /// <summary>
        /// Reads a 16 bits unsigned integer value.
        /// </summary>
        /// <returns>Returns a 16 bits unsigned integer value.</returns>
        public override ushort ReadUInt16()
        {
            if (ChangeEndianness == false)
                return base.ReadUInt16();
            return ReadAndSwap<ushort>(BitConverter.ToUInt16);
        }

        /// <summary>
        /// Reads a 32 bits unsigned integer value.
        /// </summary>
        /// <returns>Returns a 32 bits unsigned integer value.</returns>
        public override uint ReadUInt32()
        {
            if (ChangeEndianness == false)
                return base.ReadUInt32();
            return ReadAndSwap<uint>(BitConverter.ToUInt32);
        }

        /// <summary>
        /// Reads a 64 bits unsigned integer value.
        /// </summary>
        /// <returns>Returns a 64 bits unsigned integer value.</returns>
        public override ulong ReadUInt64()
        {
            if (ChangeEndianness == false)
                return base.ReadUInt64();
            return ReadAndSwap<ulong>(BitConverter.ToUInt64);
        }

        private T ReadAndSwap<T>(Func<byte[], int, T> convertionFunc)
        {
            var bytes = base.ReadBytes(Marshal.SizeOf(typeof(T)));
            Array.Reverse(bytes);
            return convertionFunc(bytes, 0);
        }
    }
}
