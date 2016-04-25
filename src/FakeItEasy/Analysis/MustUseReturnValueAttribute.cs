namespace FakeItEasy.Analysis
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal sealed class MustUseReturnValueAttribute : Attribute
    {
        public MustUseReturnValueAttribute(string diagnosticName)
        {
            this.DiagnosticName = diagnosticName;
        }

        public string DiagnosticName { get; }
    }
}
