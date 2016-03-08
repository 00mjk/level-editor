using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter
{
    /// <summary>
    /// Represent a versionable entity.
    /// </summary>
    public interface IVersionable
    {
        /// <summary>
        /// Gets the version number.
        /// </summary>
        uint Version { get; }
    }

    /// <summary>
    /// Represent a binary data reader.
    /// </summary>
    public interface IReader : IVersionable
    {
        /// <summary>
        /// Reads data from a BinaryReader instance.
        /// </summary>
        /// <param name="reader">The BinaryReader to read data from.</param>
        /// <returns>Returns the fulfilled data structure.</returns>
        object Read(BinaryReader reader);
    }

    /// <summary>
    /// Represent a binary data writer.
    /// </summary>
    public interface IWriter : IVersionable
    {
        /// <summary>
        /// Writes data to a BinaryWriter instance.
        /// </summary>
        /// <param name="data">The data structure to write.</param>
        /// <param name="writer">The BinaryWriter to write data to.</param>
        void Write(object data, BinaryWriter writer);
    }

    //public abstract class ReaderBase : IReader, IDisposable
    //{
    //    public abstract uint Version { get; }
    //    protected BinaryReader reader { get; private set; }

    //    ~ReaderBase() 
    //    {
    //        Dispose(false);
    //    }

    //    public void Initialize(BinaryReader reader)
    //    {
    //        if (reader == null)
    //            throw new ArgumentNullException("reader");

    //        this.reader = reader;
    //    }

    //    protected void CheckInitialization()
    //    {
    //        if (reader == null)
    //            throw new InvalidOperationException("Reader has not been initialized.");
    //    }

    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            if (reader != null)
    //            {
    //                if (reader.BaseStream != null)
    //                    reader.BaseStream.Dispose();

    //                ((IDisposable)reader).Dispose();
    //                reader = null;
    //            }
    //        }
    //    }
    //}

    //public abstract class WriterBase : IWriter, IDisposable
    //{
    //    public abstract uint Version { get; }
    //    protected BinaryWriter writer { get; private set; }

    //    ~WriterBase() 
    //    {
    //        Dispose(false);
    //    }

    //    public void Initialize(BinaryWriter writer)
    //    {
    //        if (writer == null)
    //            throw new ArgumentNullException("writer");

    //        this.writer = writer;
    //    }

    //    protected void CheckInitialization()
    //    {
    //        if (writer == null)
    //            throw new InvalidOperationException("Writer has not been initialized.");
    //    }

    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            if (writer != null)
    //            {
    //                if (writer.BaseStream != null)
    //                    writer.BaseStream.Dispose();

    //                ((IDisposable)writer).Dispose();
    //                writer = null;
    //            }
    //        }
    //    }
    //}
}
