namespace ResharperDiagnosticsConverter
{
    /// <summary>
    /// Offset to line column converter.
    /// </summary>
    public interface IOffsetToLineColumnConverter
    {
        /// <summary>
        /// Gets line column.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="offset">The offset.</param>
        LineColumnPosition GetLineColumn(string filename, int offset);
    }
}
