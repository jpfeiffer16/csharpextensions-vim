using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ResharperDiagnosticsConverter
{
    public class OffsetToLineColumnConverter
    {
        private Dictionary<string, List<OffsetLineMap>> _fileMappings = new Dictionary<string, List<OffsetLineMap>>();

        public (int line, int column) GetLineColumn(string filename, int offset)
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

            return (lineNumber + 1, column + 1); 
        }

        private List<OffsetLineMap> CreateMappings(string filename)
        {
            var content = File.ReadAllText(filename);
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
