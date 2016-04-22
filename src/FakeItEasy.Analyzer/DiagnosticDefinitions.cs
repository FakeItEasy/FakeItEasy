namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticDefinitions
    {
        public static DiagnosticDescriptor UnusedCallSpecification { get; } = CreateDiagnosticDescriptor(nameof(UnusedCallSpecification), "FakeItEasy0001", "FakeItEasy.Usage", DiagnosticSeverity.Error, true);

        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1305:SpecifyIFormatProvider",
            MessageId = "System.String.Format(System.String,System.Object[])",
            Justification = "Irrelevant in this case")]
        public static ImmutableDictionary<string, DiagnosticDescriptor> GetDiagnosticsMap(params string[] diagnosticNames)
        {
            if (diagnosticNames == null)
            {
                throw new ArgumentNullException(nameof(diagnosticNames));
            }

            var builder = ImmutableDictionary.CreateBuilder<string, DiagnosticDescriptor>();
            var typeInfo = typeof(DiagnosticDefinitions).GetTypeInfo();
            foreach (var diagnosticName in diagnosticNames)
            {
                var property = typeInfo.GetDeclaredProperty(diagnosticName);
                var descriptor = property?.GetValue(null) as DiagnosticDescriptor;
                if (descriptor == null)
                {
                    throw new ArgumentException($"There is no diagnostic named '{diagnosticName}'");
                }

                builder.Add(diagnosticName, descriptor);
            }

            return builder.ToImmutable();
        }

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
