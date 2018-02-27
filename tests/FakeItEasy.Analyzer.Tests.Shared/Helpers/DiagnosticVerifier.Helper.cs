namespace FakeItEasy.Analyzer.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Text;
    using Microsoft.CodeAnalysis;
#if CSHARP
    using Microsoft.CodeAnalysis.CSharp;
#endif
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
#if VISUAL_BASIC
    using Microsoft.CodeAnalysis.VisualBasic;
#endif

    /// <summary>
    /// Class for turning strings into documents and getting the diagnostics on them.
    /// All methods are static.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
#if CSHARP
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
#elif VISUAL_BASIC
        private static readonly MetadataReference VisualBasicSymbolsReference = MetadataReference.CreateFromFile(typeof(VisualBasicCompilation).Assembly.Location);
#endif
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
        private static readonly MetadataReference FakeItEasyReference = MetadataReference.CreateFromFile(typeof(Fake).Assembly.Location);

        internal static string DefaultFilePathPrefix { get; } = "Test";

        internal static string CSharpDefaultFileExt { get; } = "cs";

        internal static string VisualBasicDefaultExt { get; } = "vb";

        internal static string TestProjectName { get; } = "TestProject";

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzer and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the documents.</param>
        /// <param name="documents">The Documents that the analyzer will be run on.</param>
        /// <param name="allowCompilationErrors">Allow compiler errors.</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location.</returns>
        protected static Diagnostic[] GetSortedDiagnosticsFromDocuments(DiagnosticAnalyzer analyzer, Document[] documents, bool allowCompilationErrors)
        {
            if (documents == null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            var projects = new HashSet<Project>();
            foreach (var document in documents)
            {
                projects.Add(document.Project);
            }

            var diagnostics = new List<Diagnostic>();
            var analyzerExceptions = new List<Exception>();
            foreach (var project in projects)
            {
                var options = new CompilationWithAnalyzersOptions(
                    new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty),
                    (exception, diagnosticAnalyzer, diagnostic) => analyzerExceptions.Add(exception),
                    false,
                    true);
                var compilationWithAnalyzers = project.GetCompilationAsync().Result.WithAnalyzers(ImmutableArray.Create(analyzer), options);

                if (!allowCompilationErrors)
                {
                    AssertThatCompilationSucceeded(compilationWithAnalyzers);
                }

                var diags = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

                if (analyzerExceptions.Any())
                {
                    if (analyzerExceptions.Count == 1)
                    {
                        ExceptionDispatchInfo.Capture(analyzerExceptions[0]).Throw();
                    }
                    else
                    {
                        throw new AggregateException("Multiple exceptions thrown during analysis", analyzerExceptions);
                    }
                }

                foreach (var diag in diags)
                {
                    if (diag.Location == Location.None || diag.Location.IsInMetadata)
                    {
                        diagnostics.Add(diag);
                    }
                    else
                    {
                        for (int i = 0; i < documents.Length; i++)
                        {
                            var document = documents[i];
                            var tree = document.GetSyntaxTreeAsync().Result;
                            if (tree == diag.Location.SourceTree)
                            {
                                diagnostics.Add(diag);
                            }
                        }
                    }
                }
            }

            var results = SortDiagnostics(diagnostics);
            diagnostics.Clear();
            return results;
        }

        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">Classes in the form of a string.</param>
        /// <param name="language">The language the source code is in.</param>
        /// <returns>A Document created from the source string.</returns>
        protected static Document CreateDocument(string source, string language)
        {
            return CreateProject(new[] { source }, language).Documents.First();
        }

        /// <summary>
        /// Given an array of strings as sources and a language, turn them into a project and return the documents and spans of it.
        /// </summary>
        /// <param name="sources">Classes in the form of strings.</param>
        /// <param name="language">The language the source code is in.</param>
        /// <returns>A Tuple containing the Documents produced from the sources and their TextSpans if relevant.</returns>
        private static Document[] GetDocuments(string[] sources, string language)
        {
            if (language != LanguageNames.CSharp && language != LanguageNames.VisualBasic)
            {
                throw new ArgumentException("Unsupported Language");
            }

            var project = CreateProject(sources, language);
            var documents = project.Documents.ToArray();

            if (sources.Length != documents.Length)
            {
                throw new InvalidOperationException("Amount of sources did not match amount of Documents created");
            }

            return documents;
        }

        private static void AssertThatCompilationSucceeded(CompilationWithAnalyzers compilationWithAnalyzers)
        {
            var compilationDiagnostics = compilationWithAnalyzers.Compilation.GetDiagnostics();

            if (compilationDiagnostics.Any())
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.Append("Test code compilation failed. Error(s) encountered:");
                foreach (var diagnostic in compilationDiagnostics)
                {
                    messageBuilder.AppendLine();
                    messageBuilder.AppendFormat("  {0}", diagnostic);
                }

                throw new ArgumentException(messageBuilder.ToString());
            }
        }

        /// <summary>
        /// Create a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">Classes in the form of strings.</param>
        /// <param name="language">The language the source code is in.</param>
        /// <returns>A Project created out of the Documents created from the source strings.</returns>
#if CSHARP
        private static Project CreateProject(string[] sources, string language = LanguageNames.CSharp)
#elif VISUAL_BASIC
        private static Project CreateProject(string[] sources, string language = LanguageNames.VisualBasic)
#endif
        {
            string fileNamePrefix = DefaultFilePathPrefix;
            string fileExt = language == LanguageNames.CSharp ? CSharpDefaultFileExt : VisualBasicDefaultExt;

            var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            using (var adhocWorkspace = new AdhocWorkspace())
            {
                var solution = adhocWorkspace
                    .CurrentSolution
                    .AddProject(projectId, TestProjectName, TestProjectName, language)
#if CSHARP
                    .WithProjectCompilationOptions(
                        projectId,
                        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
#elif VISUAL_BASIC
                    .WithProjectCompilationOptions(
                        projectId,
                        new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optionStrict: OptionStrict.On))
#endif
                    .AddMetadataReference(projectId, CorlibReference)
                    .AddMetadataReference(projectId, SystemCoreReference)
#if CSHARP
                    .AddMetadataReference(projectId, CSharpSymbolsReference)
#elif VISUAL_BASIC
                    .AddMetadataReference(projectId, VisualBasicSymbolsReference)
#endif
                    .AddMetadataReference(projectId, CodeAnalysisReference)
                    .AddMetadataReference(projectId, FakeItEasyReference);

                int count = 0;
                foreach (var source in sources)
                {
                    var newFileName = fileNamePrefix + count + "." + fileExt;
                    var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                    solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                    count++;
                }

                return solution.GetProject(projectId);
            }
        }

        /// <summary>
        /// Given classes in the form of strings, their language, and an IDiagnosticAnalyzer to apply to it, return the diagnostics found in the string after converting it to a document.
        /// </summary>
        /// <param name="sources">Classes in the form of strings.</param>
        /// <param name="language">The language the source classes are in.</param>
        /// <param name="analyzer">The analyzer to be run on the sources.</param>
        /// <param name="allowCompilationErrors">Allow compiler errors.</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location.</returns>
        private static Diagnostic[] GetSortedDiagnostics(string[] sources, string language, DiagnosticAnalyzer analyzer, bool allowCompilationErrors)
        {
            return GetSortedDiagnosticsFromDocuments(analyzer, GetDocuments(sources, language), allowCompilationErrors);
        }

        /// <summary>
        /// Sort diagnostics by location in source document.
        /// </summary>
        /// <param name="diagnostics">The list of Diagnostics to be sorted.</param>
        /// <returns>An IEnumerable containing the Diagnostics in order of Location.</returns>
        private static Diagnostic[] SortDiagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            return diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        }
    }
}
