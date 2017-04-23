namespace FakeItEasy.Analyzer.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Superclass of all Unit Tests for DiagnosticAnalyzers.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
#if CSHARP
        /// <summary>
        /// Called to test a C# DiagnosticAnalyzer when applied on the single inputted string as a source.
        /// Note: input a DiagnosticResult for each Diagnostic expected.
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected"> DiagnosticResults that should appear after the analyzer is run on the source.</param>
        internal void VerifyCSharpDiagnostic(string source, params DiagnosticResult[] expected)
        {
            this.VerifyDiagnostics(new[] { source }, LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzer(), expected);
        }

        /// <summary>
        /// Get the CSharp analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>The diagnostic analyzer being tested.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It's not appropriate here")]
        protected abstract DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer();
#elif VISUAL_BASIC
        /// <summary>
        /// Called to test a VB.NET DiagnosticAnalyzer when applied on the single inputted string as a source.
        /// Note: input a DiagnosticResult for each Diagnostic expected.
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected"> DiagnosticResults that should appear after the analyzer is run on the source.</param>
        internal void VerifyVisualBasicDiagnostic(string source, params DiagnosticResult[] expected)
        {
            this.VerifyDiagnostics(new[] { source }, LanguageNames.VisualBasic, this.GetVisualBasicDiagnosticAnalyzer(), expected);
        }

        /// <summary>
        /// Get the Visual Basic analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>The diagnostic analyzer being tested.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It's not appropriate here")]
        protected abstract DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer();
#endif

        /// <summary>
        /// Checks each of the actual Diagnostics found and compares them with the corresponding DiagnosticResult in the array of expected results.
        /// Diagnostics are considered equal only if the DiagnosticResultLocation, Id, Severity, and Message of the DiagnosticResult match the actual diagnostic.
        /// </summary>
        /// <param name="actualResults">The Diagnostics found by the compiler after running the analyzer on the source code.</param>
        /// <param name="analyzer">The analyzer that was being run on the sources.</param>
        /// <param name="expectedResults">Diagnostic Results that should have appeared in the code.</param>
        private static void VerifyDiagnosticResults(IEnumerable<Diagnostic> actualResults, DiagnosticAnalyzer analyzer, params DiagnosticResult[] expectedResults)
        {
            int expectedCount = expectedResults.Count();
            int actualCount = actualResults.Count();

            if (expectedCount != actualCount)
            {
                string diagnosticsOutput = actualResults.Any() ? FormatDiagnostics(analyzer, actualResults.ToArray()) : "    NONE.";

                var message =
                    $@"Mismatch between number of diagnostics returned, expected ""{expectedCount}"" actual ""{actualCount}""

Diagnostics:
{diagnosticsOutput}
";
                Execute.Assertion.FailWith(message);
            }

            for (int i = 0; i < expectedResults.Length; i++)
            {
                var actual = actualResults.ElementAt(i);
                var expected = expectedResults[i];

                if (expected.Line == -1 && expected.Column == -1)
                {
                    if (actual.Location != Location.None)
                    {
                        var message =
                            $@"Expected:
A project diagnostic with No location
Actual:
{FormatDiagnostics(analyzer, actual)}";
                        Execute.Assertion.FailWith(message);
                    }
                }
                else
                {
                    VerifyDiagnosticLocation(analyzer, actual, actual.Location, expected.Locations.First());
                    var additionalLocations = actual.AdditionalLocations.ToArray();

                    if (additionalLocations.Length != expected.Locations.Length - 1)
                    {
                        var message =
                            $@"Expected {expected.Locations.Length - 1} additional locations but got {additionalLocations.Length} for Diagnostic:
    {FormatDiagnostics(analyzer, actual)}
";
                        Execute.Assertion.FailWith(message);
                    }

                    for (int j = 0; j < additionalLocations.Length; ++j)
                    {
                        VerifyDiagnosticLocation(analyzer, actual, additionalLocations[j], expected.Locations[j + 1]);
                    }
                }

                if (actual.Id != expected.Id)
                {
                    var message =
                        $@"Expected diagnostic id to be ""{expected.Id}"" was ""{actual.Id}""

Diagnostic:
    {FormatDiagnostics(analyzer, actual)}
";
                    Execute.Assertion.FailWith(message);
                }

                if (actual.Severity != expected.Severity)
                {
                    var message =
                            $@"Expected diagnostic severity to be ""{expected.Severity}"" was ""{actual.Severity}""

Diagnostic:
    {FormatDiagnostics(analyzer, actual)}
";
                    Execute.Assertion.FailWith(message);
                }

                if (actual.GetMessage() != expected.Message)
                {
                    var message =
                            $@"Expected diagnostic message to be ""{expected.Message}"" was ""{actual.GetMessage()}""

Diagnostic:
    {FormatDiagnostics(analyzer, actual)}
";
                    Execute.Assertion.FailWith(message);
                }
            }
        }

        /// <summary>
        /// Helper method to VerifyDiagnosticResult that checks the location of a diagnostic and compares it with the location in the expected DiagnosticResult.
        /// </summary>
        /// <param name="analyzer">The analyzer that was being run on the sources.</param>
        /// <param name="diagnostic">The diagnostic that was found in the code.</param>
        /// <param name="actual">The Location of the Diagnostic found in the code.</param>
        /// <param name="expected">The DiagnosticResultLocation that should have been found.</param>
        private static void VerifyDiagnosticLocation(DiagnosticAnalyzer analyzer, Diagnostic diagnostic, Location actual, DiagnosticResultLocation expected)
        {
            var actualSpan = actual.GetLineSpan();

            var message =
                    $@"Expected diagnostic to be in file ""{expected.Path}"" was actually in file ""{actualSpan.Path}""

Diagnostic:
    {FormatDiagnostics(analyzer, diagnostic)}
";
            (actualSpan.Path == expected.Path || (actualSpan.Path != null && actualSpan.Path.Contains("Test0.") && expected.Path.Contains("Test.")))
                .Should().BeTrue(message);

            var actualLinePosition = actualSpan.StartLinePosition;

            // Only check line position if there is an actual line in the real diagnostic
            if (actualLinePosition.Line > 0)
            {
                if (actualLinePosition.Line + 1 != expected.Line)
                {
                    message =
                            $@"Expected diagnostic to be on line ""{expected.Line}"" was actually on line ""{actualLinePosition.Line + 1}""

Diagnostic:
    {FormatDiagnostics(analyzer, diagnostic)}
";
                    Execute.Assertion.FailWith(message);
                }
            }

            // Only check column position if there is an actual column position in the real diagnostic
            if (actualLinePosition.Character > 0)
            {
                if (actualLinePosition.Character + 1 != expected.Column)
                {
                    message =
                            $@"Expected diagnostic to start at column ""{expected.Column}"" was actually at column ""{actualLinePosition.Character + 1}""

Diagnostic:
    {FormatDiagnostics(analyzer, diagnostic)}
";
                    Execute.Assertion.FailWith(message);
                }
            }
        }

        /// <summary>
        /// Helper method to format a Diagnostic into an easily readable string.
        /// </summary>
        /// <param name="analyzer">The analyzer that this verifier tests.</param>
        /// <param name="diagnostics">The Diagnostics to be formatted.</param>
        /// <returns>The Diagnostics formatted as a string.</returns>
        private static string FormatDiagnostics(DiagnosticAnalyzer analyzer, params Diagnostic[] diagnostics)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < diagnostics.Length; ++i)
            {
                builder.AppendLine("// " + diagnostics[i].ToString());

                var analyzerType = analyzer.GetType();
                var rules = analyzer.SupportedDiagnostics;

                foreach (var rule in rules)
                {
                    if (rule != null && rule.Id == diagnostics[i].Id)
                    {
                        var location = diagnostics[i].Location;
                        if (location == Location.None)
                        {
                            builder.Append($"GetGlobalResult({analyzerType.Name}.{rule.Id})");
                        }
                        else
                        {
                            location.IsInSource.Should()
                                .BeTrue(
                                    $"Test base does not currently handle diagnostics in metadata locations. Diagnostic in metadata: {diagnostics[i]}\r\n");

                            string resultMethodName = diagnostics[i].Location.SourceTree.FilePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ? "GetCSharpResultAt" : "GetBasicResultAt";
                            var linePosition = diagnostics[i].Location.GetLineSpan().StartLinePosition;

                            builder.Append(
                                    $"{resultMethodName}({linePosition.Line + 1}, {linePosition.Character + 1}, {analyzerType.Name}.{rule.Id})");
                        }

                        if (i != diagnostics.Length - 1)
                        {
                            builder.Append(',');
                        }

                        builder.AppendLine();
                        break;
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// General method that gets a collection of actual diagnostics found in the source after the analyzer is run,
        /// then verifies each of them.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on.</param>
        /// <param name="language">The language of the classes represented by the source strings.</param>
        /// <param name="analyzer">The analyzer to be run on the source code.</param>
        /// <param name="expected">DiagnosticResults that should appear after the analyzer is run on the sources.</param>
        private void VerifyDiagnostics(string[] sources, string language, DiagnosticAnalyzer analyzer, params DiagnosticResult[] expected)
        {
            var diagnostics = GetSortedDiagnostics(sources, language, analyzer);
            VerifyDiagnosticResults(diagnostics, analyzer, expected);
        }
    }
}
