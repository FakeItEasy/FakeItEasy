namespace FakeItEasy.Analyzer
{
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticDefinitions
    {
        public static DiagnosticDescriptor UnusedCallSpecification { get; } =
            CreateDiagnosticDescriptor(
                nameof(UnusedCallSpecification), "FakeItEasy0001", "FakeItEasy.Usage", DiagnosticSeverity.Error, true);

        public static DiagnosticDescriptor NonVirtualSetupSpecification { get; } =
            CreateDiagnosticDescriptor(
                nameof(NonVirtualSetupSpecification), "FakeItEasy0002", "FakeItEasy.Usage", DiagnosticSeverity.Warning, true);

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
            return new LocalizableResourceString(name + propertyName, Resources.ResourceManager, typeof(Resources));
        }
    }
}
