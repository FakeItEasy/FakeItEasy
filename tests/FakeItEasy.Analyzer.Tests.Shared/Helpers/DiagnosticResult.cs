namespace FakeItEasy.Analyzer.Tests.Helpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Location where the diagnostic appears, as determined by path, line number, and column number.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Not necessary")]
    internal struct DiagnosticResultLocation
    {
        public DiagnosticResultLocation(string path, int line, int column)
        {
            if (line < 0 && column < 0)
            {
                throw new ArgumentException("At least one of line and column must be > 0");
            }

            if (line < -1 || column < -1)
            {
                throw new ArgumentException("Both line and column must be >= -1");
            }

            this.Path = path;
            this.Line = line;
            this.Column = column;
        }

        public string Path { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }
    }

    /// <summary>
    /// Struct that stores information about a Diagnostic appearing in a source.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Not necessary")]
    internal struct DiagnosticResult
    {
        private DiagnosticResultLocation[] locations;

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "It's not intended to be a public API, so it doesn't matter")]
        public DiagnosticResultLocation[] Locations
        {
            get
            {
                if (this.locations == null)
                {
                    this.locations = Array.Empty<DiagnosticResultLocation>();
                }

                return this.locations;
            }

            set
            {
                this.locations = value;
            }
        }

        public DiagnosticSeverity Severity { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

        public int Line => this.Locations.Length > 0 ? this.Locations[0].Line : -1;

        public int Column => this.Locations.Length > 0 ? this.Locations[0].Column : -1;
    }
}
