using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter.Conversion
{
    /// <summary>
    /// Represents a data format converter.
    /// </summary>
    public interface IFormatConverter
    {
        /// <summary>
        /// Gets the version the current IFormatConverter converts from.
        /// </summary>
        uint From { get; }

        /// <summary>
        /// Gets the version the current IFormatConverter converts to.
        /// </summary>
        uint To { get; }

        /// <summary>
        /// Converts input data.
        /// </summary>
        /// <param name="inputData">Input data to convert.</param>
        /// <returns>Returns the converted data.</returns>
        object Convert(object inputData);
    }
}
