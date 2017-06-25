namespace FakeItEasy.Analyzer.VisualBasic.Tests
{
    using FakeItEasy.Analyzer.Tests.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    public class ArgumentConstraintNullabilityMismatchAnalyzerTests : CodeFixVerifier
    {
        private const string CodeTemplate = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub TheTest()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(Sub() {0}).DoesNothing()
        End Sub
    End Class

    Interface IFoo
        Sub NullableParam(ByVal x As Integer?)
        Sub OtherNullableParam(ByVal x As Long?)
        Sub NonNullableParam(ByVal x As Integer)
        Sub OtherNonNullableParam(ByVal x As Long?)
    End Interface
End Namespace";

        [Fact]
        public void Diagnostic_should_be_triggered_when_non_nullable_constraint_is_used_for_nullable_parameter()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo.NullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0004",
                    Message = $"Parameter 'x' of method 'NullableParam' is nullable, but argument constraint '{completeConstraint}' is not. Calls where this argument is Nothing will not be matched.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 46) }
                });
        }

        [Fact]
        public void Diagnostic_should_not_be_triggered_for_That_constraint()
        {
            string completeConstraint = "A(Of Integer).That.Matches(Function(x) True)";
            string call = $"foo.NullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Fact]
        public void Diagnostic_should_not_be_triggered_when_nullable_constraint_is_used_for_nullable_parameter()
        {
            string completeConstraint = "A(Of Integer?).Ignored";
            string call = $"foo.NullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Fact]
        public void Diagnostic_should_not_be_triggered_when_non_nullable_constraint_is_used_for_non_nullable_parameter()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo.NonNullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Fact]
        public void Diagnostic_should_not_be_triggered_when_non_nullable_constraint_is_used_for_other_nullable_parameter()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo.OtherNullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Fact]
        public void Diagnostic_should_not_be_triggered_when_nullable_constraint_is_used_for_other_nullable_parameter()
        {
            string completeConstraint = "A(Of Integer?).Ignored";
            string call = $"foo.OtherNullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Fact]
        public void Diagnostic_should_not_be_triggered_when_non_nullable_constraint_is_used_for_other_non_nullable_parameter()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo.OtherNonNullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Fact]
        public void MakeConstraintNullable_CodeFix_should_replace_constraint_with_nullable_constraint()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo.NullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A(Of Integer?).Ignored";
            string fixedCall = $"foo.NullableParam({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 0);
        }

        [Fact]
        public void MakeNotNullConstraint_CodeFix_should_replace_constraint_with_AThatIsNotNull()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo.NullableParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A(Of Integer?).That.IsNotNull()";
            string fixedCall = $"foo.NullableParam({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 1);
        }

        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer()
        {
            return new ArgumentConstraintNullabilityMismatchAnalyzer();
        }

        protected override CodeFixProvider GetVisualBasicCodeFixProvider()
        {
            return new ArgumentConstraintNullabilityMismatchCodeFixProvider();
        }
    }
}
