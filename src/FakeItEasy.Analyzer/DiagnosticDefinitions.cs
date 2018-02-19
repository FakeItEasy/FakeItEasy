namespace FakeItEasy.Analyzer
{
    using System.Reflection;
    using System.Resources;
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticDefinitions
    {
        internal static readonly ResourceManager ResourceManager =
            new ResourceManager(
                ResourceBaseName,
                typeof(DiagnosticDefinitions).GetTypeInfo().Assembly);

#if CSHARP
        private const string ResourceBaseName = "FakeItEasy.Analyzer.CSharp.Resources";
#elif VISUAL_BASIC
        private const string ResourceBaseName = "FakeItEasy.Analyzer.VisualBasic.Resources";
#endif

        public static DiagnosticDescriptor UnusedCallSpecification { get; } =
            CreateDiagnosticDescriptor(
                nameof(UnusedCallSpecification),
                "FakeItEasy0001",
                "FakeItEasy.Usage",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor NonVirtualSetupSpecification { get; } =
            CreateDiagnosticDescriptor(
                nameof(NonVirtualSetupSpecification),
                "FakeItEasy0002",
                "FakeItEasy.Usage",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor ArgumentConstraintOutsideCallSpec { get; } =
            CreateDiagnosticDescriptor(
                nameof(ArgumentConstraintOutsideCallSpec),
                "FakeItEasy0003",
                "FakeItEasy.Usage",
                DiagnosticSeverity.Warning,
                true);

        public static DiagnosticDescriptor ArgumentConstraintNullabilityMismatch { get; } =
            CreateDiagnosticDescriptor(
                nameof(ArgumentConstraintNullabilityMismatch),
                "FakeItEasy0004",
                "FakeItEasy.Usage",
                DiagnosticSeverity.Warning,
                true);

        public static DiagnosticDescriptor ArgumentConstraintTypeMismatch { get; } =
            CreateDiagnosticDescriptor(
                nameof(ArgumentConstraintTypeMismatch),
                "FakeItEasy0005",
                "FakeItEasy.Usage",
                DiagnosticSeverity.Error,
                true);

        public static DiagnosticDescriptor RepeatedAssertion { get; } =
            CreateDiagnosticDescriptor(
                nameof(RepeatedAssertion),
                "FakeItEasy0006",
                "FakeItEasy.Usage",
                DiagnosticSeverity.Warning,
                true);

        private static DiagnosticDescriptor CreateDiagnosticDescriptor(
            string name, string id, string category, DiagnosticSeverity defaultSeverity, bool isEnabledByDefault)
        {
            var title = GetDiagnosticResourceString(name, nameof(DiagnosticDescriptor.Title));
            var messageFormat = GetDiagnosticResourceString(name, nameof(DiagnosticDescriptor.MessageFormat));
            var description = GetDiagnosticResourceString(name, nameof(DiagnosticDescriptor.Description));
            return new DiagnosticDescriptor(id, title, messageFormat, category, defaultSeverity, isEnabledByDefault, description);
        }

        private static LocalizableResourceString GetDiagnosticResourceString(string name, string propertyName)
        {
            return new LocalizableResourceString(name + propertyName, ResourceManager, typeof(DiagnosticDefinitions));
        }
    }
}
