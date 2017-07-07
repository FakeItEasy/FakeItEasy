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
            A.CallTo(Function() {0}).Returns(42)
        End Sub
    End Class

    Interface IFoo
        Function NullableParam(ByVal x As Integer?) As Integer
        Function OtherNullableParam(ByVal x As Long?) As Integer
        Function NonNullableParam(ByVal x As Integer) As Integer
        Function OtherNonNullableParam(ByVal x As Long?) As Integer
        Default Readonly Property Item(ByVal x As Integer?) As Integer
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
                    Message = $"Parameter 'x' of method or indexer 'NullableParam' is nullable, but argument constraint '{completeConstraint}' is not. Calls where this argument is Nothing will not be matched.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 51) }
                });
        }

        [Fact]
        public void Diagnostic_should_be_triggered_when_non_nullable_constraint_is_used_for_nullable_parameter_for_indexer()
        {
            string completeConstraint = $"A(Of Integer).Ignored";
            string call = $"foo({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0004",
                    Message = $"Parameter 'x' of method or indexer 'Item' is nullable, but argument constraint '{completeConstraint}' is not. Calls where this argument is Nothing will not be matched.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 37) }
                });
        }

        [Fact]
        public void Diagnostic_should_be_triggered_when_non_nullable_constraint_is_used_for_nullable_parameter_for_named_indexer()
        {
            string completeConstraint = $"A(Of Integer).Ignored";
            string call = $"foo.Item({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0004",
                    Message = $"Parameter 'x' of method or indexer 'Item' is nullable, but argument constraint '{completeConstraint}' is not. Calls where this argument is Nothing will not be matched.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 42) }
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
        public void MakeConstraintNullable_CodeFix_should_replace_constraint_with_nullable_constraint_for_indexer()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A(Of Integer?).Ignored";
            string fixedCall = $"foo({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 0);
        }

        [Fact]
        public void MakeConstraintNullable_CodeFix_should_replace_constraint_with_nullable_constraint_for_named_indexer()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo.Item({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A(Of Integer?).Ignored";
            string fixedCall = $"foo.Item({fixedConstraint})";
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

        [Fact]
        public void MakeNotNullConstraint_CodeFix_should_replace_constraint_with_AThatIsNotNull_for_indexer()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A(Of Integer?).That.IsNotNull()";
            string fixedCall = $"foo({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 1);
        }

        [Fact]
        public void MakeNotNullConstraint_CodeFix_should_replace_constraint_with_AThatIsNotNull_for_named_indexer()
        {
            string completeConstraint = "A(Of Integer).Ignored";
            string call = $"foo.Item({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A(Of Integer?).That.IsNotNull()";
            string fixedCall = $"foo.Item({fixedConstraint})";
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
