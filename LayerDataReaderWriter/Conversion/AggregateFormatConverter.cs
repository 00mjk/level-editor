using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayerDataReaderWriter.Conversion
{
    /// <summary>
    /// Represent a cascading composition of several FormatConverters.
    /// </summary>
    public class AggregateFormatConverter : IFormatConverter
    {
        /// <summary>
        /// Gets the version the current IFormatConverter converts from.
        /// </summary>
        public uint From { get; private set; }

        /// <summary>
        /// Gets the version the current IFormatConverter converts to.
        /// </summary>
        public uint To { get; private set; }

        private IFormatConverter[] conversionPath;

        /// <summary>
        /// Initializes the AggregateFormatConverter instance.
        /// </summary>
        /// <param name="from">The version the current IFormatConverter converts from.</param>
        /// <param name="to">The version the current IFormatConverter converts to.</param>
        /// <param name="knownFormatConverters">An array of aggregated FormatConverter instances.</param>
        public AggregateFormatConverter(uint from, uint to, IFormatConverter[] knownFormatConverters)
        {
            if (to <= from)
                throw new ArgumentException("Invalid 'from' and 'to' combination");
            if (knownFormatConverters == null)
                throw new ArgumentNullException("knownFormatConverters");

            From = from;
            To = to;

            var earlyTest = knownFormatConverters.FirstOrDefault(x => x.From == from && x.To == to);
            if (earlyTest != null)
            {
                conversionPath = new[] { earlyTest };
                return;
            }

            var root = ConstructTree(From, null, knownFormatConverters);
            conversionPath = FindShortestTo(To, root, knownFormatConverters).ToArray();
        }

        /// <summary>
        /// Converts input data.
        /// </summary>
        /// <param name="inputData">Input data to convert.</param>
        /// <returns>Returns the converted data.</returns>
        public object Convert(object inputData)
        {
            foreach (var converter in conversionPath)
                inputData = converter.Convert(inputData);

            return inputData;
        }

        private Node ConstructTree(uint from, IFormatConverter converter, IFormatConverter[] converters)
        {
            var node = new Node
            {
                Converter = converter,
                To = converters
                    .Where(c => c.From == from)
                    .Select(c => ConstructTree(c.To, c, converters))
                    .ToArray()
            };

            foreach (var n in node.To)
                n.Parent = node;

            return node;
        }

        private IEnumerable<IFormatConverter> FindShortestTo(uint version, Node root, IFormatConverter[] converters)
        {
            var paths = new HashSet<string>();

            FindAllPathsTo(version, root, paths);

            var shortestPath = paths
                .Select(x => x.Split(','))
                .OrderBy(p => p.Length)
                .FirstOrDefault();

            if (shortestPath == null || shortestPath.Length < 2)
                throw new InvalidOperationException(string.Format("Cannot find conversion path from version '{0}' to '{1}'", From, version));

            var numPath = shortestPath.Select(uint.Parse).ToArray();
            uint from = numPath[0];

            foreach (var to in numPath.Skip(1))
            {
                var next = converters.FirstOrDefault(c => c.From == from && c.To == to);
                if (next == null)
                    throw new InvalidOperationException(string.Format("Converter from '{0}' to '{1}' is unexpectedly missing", from, to));

                yield return next;

                from = to;
            }
        }

        private void FindAllPathsTo(uint to, Node node, HashSet<string> paths)
        {
            if (node.Converter != null && node.Converter.To == to)
                paths.Add(GetPathString(node));

            foreach (var n in node.To)
                FindAllPathsTo(to, n, paths);
        }

        private string GetPathString(Node node)
        {
            if (node.Parent.Converter == null)
                return string.Format("{0},{1}", node.Converter.From, node.Converter.To);
            return GetPathString(node.Parent) + "," + node.Converter.To;
        }

        private class Node
        {
            public Node Parent;
            public IFormatConverter Converter;
            public Node[] To;
        }
    }
}
