namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticDefinitions
    {
        public static DiagnosticDescriptor UnusedCallSpecification { get; } = CreateDiagnosticDescriptor(nameof(UnusedCallSpecification), "FakeItEasy0001", "FakeItEasy.Usage", DiagnosticSeverity.Error, true);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object[])", Justification = "Irrelevant in this case")]
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

        private static DiagnosticDescriptor CreateDiagnosticDescriptor(string diagnosticName, string diagnosticId, string category, DiagnosticSeverity severity, bool isEnabledByDefault)
        {
            var title = GetDiagnosticResourceString(diagnosticName, nameof(DiagnosticDescriptor.Title));
            var messageFormat = GetDiagnosticResourceString(diagnosticName, nameof(DiagnosticDescriptor.MessageFormat));
            var description = GetDiagnosticResourceString(diagnosticName, nameof(DiagnosticDescriptor.Description));
            return new DiagnosticDescriptor(diagnosticId, title, messageFormat, category, severity, isEnabledByDefault, description);
        }

        private static LocalizableResourceString GetDiagnosticResourceString(string diagnosticName, string suffix)
        {
            return new LocalizableResourceString(diagnosticName + suffix, Resources.ResourceManager, typeof(Resources));
        }
    }
}
