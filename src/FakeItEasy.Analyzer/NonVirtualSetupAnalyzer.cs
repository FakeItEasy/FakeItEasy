using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FakeItEasy.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class NonVirtualSetupAnalyzer:DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
    }
}
