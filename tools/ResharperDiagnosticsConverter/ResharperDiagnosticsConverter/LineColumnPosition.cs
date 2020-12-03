namespace ResharperDiagnosticsConverter
{
    /// <summary>
    /// Line column position.
    /// </summary>
    public class LineColumnPosition
    {
        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public int Line { get; }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public int Column { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineColumnPosition"/> class.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        public LineColumnPosition(int line, int column)
        {
            Line = line;
            Column = column;
        }
    }
}
