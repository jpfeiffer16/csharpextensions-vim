using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Abstractions;

namespace ResharperDiagnosticsConverter
{
    /// <inheritdoc />
    public class OffsetToLineColumnConverter : IOffsetToLineColumnConverter
    {
        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<string, List<OffsetLineMap>> _fileMappings =
            new Dictionary<string, List<OffsetLineMap>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetToLineColumnConverter"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        public OffsetToLineColumnConverter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem
                ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        /// <summary>
        /// Gets line column.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="offset">The offset.</param>
        public LineColumnPosition GetLineColumn(string filename, int offset)
        {
            if (!_fileMappings.ContainsKey(filename))
            {
                _fileMappings[filename] = CreateMappings(filename);
            }
            var line = _fileMappings[filename].FirstOrDefault(m => m.Start <= offset && m.End >= offset);
            if (line is null)
                throw new Exception("line not found");

            var lineNumber = _fileMappings[filename].IndexOf(line);
            var column = offset - line.Start;

            return new LineColumnPosition(lineNumber + 1, column + 1);
        }

        private List<OffsetLineMap> CreateMappings(string filename)
        {
            var content = _fileSystem.File.ReadAllText(filename);
            var mappings = new List<OffsetLineMap>();

            mappings.Add(new OffsetLineMap { Start = 0 });

            int i = 0;
            for (; i < content.Length; i++)
            {
                var ch = content[i];
                if (ch == '\n')
                {
                    mappings.LastOrDefault().End = i;
                    mappings.Add(new OffsetLineMap { Start = i + 1 });
                }
            }

            if (mappings.LastOrDefault().End == 0)
            {
                mappings.LastOrDefault().End = i;
            }

            return mappings;
        }
    }
}
