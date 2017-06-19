namespace FakeItEasy.Analyzer.CSharp.Tests
{
    using System.Collections.Generic;
    using FakeItEasy.Analyzer.Tests.Helpers;
    using FakeItEasy.Tests;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    public class ArgumentConstraintNullabilityMismatchAnalyzerTests : DiagnosticVerifier
    {
        private const string CodeTemplate = @"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void TheTest()
        {{
            var foo = A.Fake<IFoo>();
            A.CallTo(() => {0}).DoesNothing();
        }}
    }}

    interface IFoo
    {{
        void NullableParam(int? x);
        void OtherNullableParam(long? x);
        void NonNullableParam(int x);
        void OtherNonNullableParam(int x);
    }}
}}";

        public static IEnumerable<object[]> SupportedConstraints =>
            TestCases.FromObject(
                "_",
                "Ignored");

        [Theory]
        [MemberData(nameof(SupportedConstraints))]
        public void Diagnostic_should_be_triggered_when_non_nullable_constraint_is_used_for_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo.NullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0004",
                    Message = $"Parameter 'x' of method 'NullableParam' is nullable, but argument constraint '{completeConstraint}' is not. Calls where this argument is null will not be matched.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 46) }
                });
        }

        [Fact]
        public void Diagnostic_should_not_be_triggered_for_That_constraint()
        {
            string completeConstraint = "A<int>.That.Matches(x => true)";
            string call = $"foo.NullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(SupportedConstraints))]
        public void Diagnostic_should_not_be_triggered_when_nullable_constraint_is_used_for_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int?>.{constraint}";
            string call = $"foo.NullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(SupportedConstraints))]
        public void Diagnostic_should_not_be_triggered_when_non_nullable_constraint_is_used_for_non_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo.NonNullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(SupportedConstraints))]
        public void Diagnostic_should_not_be_triggered_when_non_nullable_constraint_is_used_for_other_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo.OtherNullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(SupportedConstraints))]
        public void Diagnostic_should_not_be_triggered_when_nullable_constraint_is_used_for_other_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int?>.{constraint}";
            string call = $"foo.OtherNullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(SupportedConstraints))]
        public void Diagnostic_should_not_be_triggered_when_non_nullable_constraint_is_used_for_other_non_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo.OtherNonNullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ArgumentConstraintNullabilityMismatchAnalyzer();
        }
    }
}
