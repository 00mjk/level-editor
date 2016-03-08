using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LayerDataReaderWriter.Conversion;
using LayerDataReaderWriter.V1;
using LayerDataReaderWriter.V2;
using LayerDataReaderWriter.V3;
using LayerDataReaderWriter.V4;
using LayerDataReaderWriter.V5;
using LayerDataReaderWriter.V6;
using LayerDataReaderWriter.V7;
using LayerDataReaderWriter.V8;
using LayerDataReaderWriter.V9;

namespace LayerDataReaderWriter
{
    /// <summary>
    /// Represents a container for readers and writers.
    /// </summary>
    public static class ReaderWriterManager
    {
        /// <summary>
        /// A magic number that tells about endianness.
        /// </summary>
        public const uint MagicNumber = 0xFFFEFDFC;

        private static IReader[] readers = new IReader[]
        {
            new ReaderV1(),
            new ReaderV2(),
            new ReaderV3(),
            new ReaderV4(),
            new ReaderV5(),
            new ReaderV6(),
            new ReaderV7(),
            new ReaderV8(),
            new ReaderV9(),
        };

        private static IWriter[] writers = new IWriter[]
        {
            new WriterV1(),
            new WriterV2(),
            new WriterV3(),
            new WriterV4(),
            new WriterV5(),
            new WriterV6(),
            new WriterV7(),
            new WriterV8(),
            new WriterV9(),
        };

        private static IFormatConverter[] converters = new IFormatConverter[]
        {
            new V2ToV3FormatConverter(),
            new V3ToV4FormatConverter(),
            new V4ToV5FormatConverter(),
            new V5ToV6FormatConverter(),
            new V6ToV7FormatConverter(),
            new V7ToV8FormatConverter(),
            new V8ToV9FormatConverter(),
        };

        /// <summary>
        /// Reads a data structure from a stream with no version knowledge.
        /// </summary>
        /// <param name="readStream">The stream to read data from.</param>
        /// <param name="encoding">The encoding used when reading data.</param>
        /// <returns>Returns a typed and versioned fulfilled data structure.</returns>
        public static object Read(Stream readStream, Encoding encoding)
        {
            return ReadInternal(readStream, encoding, false, 0);
        }

        /// <summary>
        /// Reads a data structure from a stream with no version knowledge but with a target version requirement.
        /// </summary>
        /// <param name="readStream">The stream to read data from.</param>
        /// <param name="encoding">The encoding used when reading data.</param>
        /// <param name="targetVersion">The desired version to get.</param>
        /// <returns>Returns a typed and versioned fulfilled data structure.</returns>
        /// <remarks>If the stream contains data from a different version, the data will be automatically converted to the required version.
        /// However, if the target version is lower than the version contained in the stream, an ArgumentException is thrown.</remarks>
        public static object Read(Stream readStream, Encoding encoding, uint targetVersion)
        {
            if (readers.Any(x => x.Version == targetVersion) == false)
                return null;

            return ReadInternal(readStream, encoding, true, targetVersion);
        }

        private static object ReadInternal(Stream readStream, Encoding encoding, bool isRequestingSpecificVersion, uint targetVersion)
        {
            if (readStream == null)
                throw new ArgumentNullException("readStream");

            if (encoding == null)
                encoding = Encoding.UTF8;

            CustomBinaryReader br = null;

            try
            {
                br = new CustomBinaryReader(readStream, encoding);
                var magicNumber = br.ReadBytes(4);

                if (BitConverter.ToUInt32(magicNumber, 0) != MagicNumber) // not matching current endianness
                {
                    Array.Reverse(magicNumber);
                    if (BitConverter.ToUInt32(magicNumber, 0) != MagicNumber) // not matching this endianness either, file corrupted or not a layer data file
                        return null;

                    br.ChangeEndianness = true;
                }

                uint sourceVersion = br.ReadUInt32();

                var sourceReader = readers.FirstOrDefault(x => x.Version == sourceVersion);
                if (sourceReader == null)
                    return null;

                var data = sourceReader.Read(br);

                if (isRequestingSpecificVersion == false || sourceVersion == targetVersion)
                    return data;

                var converter = new AggregateFormatConverter(sourceVersion, targetVersion, converters);
                return converter.Convert(data);
            }
            catch
            {
                if (br != null)
                    ((IDisposable)br).Dispose();
                throw;
            }
        }

        /// <summary>
        /// Writes a data structure to a stream.
        /// </summary>
        /// <param name="data">The typed data structure to write.</param>
        /// <param name="writeStream">The stream to write data to.</param>
        /// <param name="encoding">The encoding used when writing data.</param>
        /// <param name="version">The version of the data to write.</param>
        public static void Write(object data, Stream writeStream, Encoding encoding, uint version)
        {
            if (writeStream == null)
                throw new ArgumentNullException("writeStream");

            if (encoding == null)
                encoding = Encoding.UTF8;

            var writer = writers.FirstOrDefault(x => x.Version == version);
            if (writer == null)
                throw new InvalidOperationException(string.Format("Writer version '{0}' not found", version));

            BinaryWriter bw = null;

            try
            {
                bw = new BinaryWriter(writeStream, encoding);

                bw.Write(MagicNumber);
                bw.Write(version);

                writer.Write(data, bw);
            }
            catch
            {
                if (bw != null)
                    ((IDisposable)bw).Dispose();
                throw;
            }
        }
    }
}
