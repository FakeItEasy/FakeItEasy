namespace FakeItEasy.Analyzer.VisualBasic.Tests
{
    using System.Collections.Generic;
    using FakeItEasy.Analyzer.Tests.Helpers;
    using FakeItEasy.Tests;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    public class ArgumentConstraintTypeMismatchAnalyzerTests : CodeFixVerifier
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
        Function NullableIntParam(ByVal x As Integer?) As Integer
        Function NullableLongParam(ByVal x As Long?) As Integer
        Function NonNullableIntParam(ByVal x As Integer) As Integer
        Function NonNullableDoubleParam(ByVal x As Double) As Integer
        Function BaseClassParam(ByVal x as BaseClass) As Integer
        Function HasImplicitConversionParam(ByVal x as CanBeConvertedTo) as Integer
        Default Readonly Property Item(ByVal x As Integer?) As Integer
        Default Readonly Property Item(ByVal x As Double, ByVal y as String) As Integer
    End Interface

    Class BaseClass
    End Class

    Class DerivedClass
        Inherits BaseClass
    End Class
End Namespace

    Class CanBeConvertedTo
    End Class

    Class CanBeConvertedFrom
        Public Shared Widening Operator CType(ByVal x As CanBeConvertedFrom) As CanBeConvertedTo
            return new CanBeConvertedTo()
        End Operator
    End Class
";

        public static IEnumerable<object[]> FakeItEasy0004SupportedConstraints =>
            TestCases.FromObject(
                "Ignored");

        public static IEnumerable<object[]> FakeItEasy0005SupportedConstraints =>
            TestCases.FromObject(
                "Ignored",
                "That.Matches(Function(anything) true)");

        public static IEnumerable<object[]> AllSupportedConstraints => FakeItEasy0005SupportedConstraints;

        [Theory]
        [MemberData(nameof(AllSupportedConstraints))]
        public void No_diagnostic_should_be_triggered_when_nullable_constraint_is_used_for_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A(Of Integer?).{constraint}";
            string call = $"foo.NullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(AllSupportedConstraints))]
        public void No_diagnostic_should_be_triggered_when_non_nullable_constraint_is_used_for_non_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A(Of Integer).{constraint}";
            string call = $"foo.NonNullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(AllSupportedConstraints))]
        public void No_diagnostic_should_be_triggered_when_derived_nonnullable_class_constraint_is_used_with_base_class_parameter(string constraint)
        {
            string completeConstraint = $"A(Of DerivedClass).{constraint}";
            string call = $"foo.BaseClassParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void FakeItEasy0004_should_be_triggered_when_non_nullable_constraint_is_used_for_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A(Of Integer).{constraint}";
            string call = $"foo.NullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0004",
                    Message = $"Parameter 'x' of method or indexer 'NullableIntParam' is nullable, but argument constraint '{completeConstraint}' is not. Calls where this argument is Nothing will not be matched.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 54) }
                });
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void FakeItEasy0004_should_be_triggered_when_non_nullable_constraint_is_used_for_nullable_parameter_for_indexer(string constraint)
        {
            string completeConstraint = $"A(Of Integer).{constraint}";
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

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void FakeItEasy0005_should_be_triggered_when_wrong_typed_nonnullable_constraint_is_used_with_nonnullable_parameter(string constraint)
        {
            string completeConstraint = $"A(Of Integer).{constraint}";
            string call = $"foo.NonNullableDoubleParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0005",
                    Message = $"Parameter 'x' of method or indexer 'NonNullableDoubleParam' has type Double, but argument constraint '{completeConstraint}' has type Integer. No calls can match this constraint.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 60) }
                });
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void FakeItEasy0005_should_be_triggered_when_wrong_typed_nullable_constraint_is_used_with_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A(Of Integer?).{constraint}";
            string call = $"foo.NullableLongParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0005",
                    Message = $"Parameter 'x' of method or indexer 'NullableLongParam' has type Long?, but argument constraint '{completeConstraint}' has type Integer?. No calls can match this constraint.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 55) }
                });
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void FakeItEasy0005_should_be_triggered_when_constraint_with_implicit_conversion_is_used_with_target_type(string constraint)
        {
            string completeConstraint = $"A(Of CanBeConvertedFrom).{constraint}";
            string call = $"foo.HasImplicitConversionParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyVisualBasicDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0005",
                    Message = $"Parameter 'x' of method or indexer 'HasImplicitConversionParam' has type CanBeConvertedTo, but argument constraint '{completeConstraint}' has type CanBeConvertedFrom. No calls can match this constraint.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 64) }
                });
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void MakeConstraintNullable_CodeFix_should_replace_constraint_with_nullable_constraint(string constraint)
        {
            string completeConstraint = $"A(Of Integer).{constraint}";
            string call = $"foo.NullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = $"A(Of Integer?).{constraint}";
            string fixedCall = $"foo.NullableIntParam({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 0);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void MakeConstraintNullable_CodeFix_should_replace_constraint_with_nullable_constraint_for_indexer(string constraint)
        {
            string completeConstraint = $"A(Of Integer).{constraint}";
            string call = $"foo.Item({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = $"A(Of Integer?).{constraint}";
            string fixedCall = $"foo.Item({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 0);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void MakeNotNullConstraint_CodeFix_should_replace_constraint_with_AThatIsNotNull(string constraint)
        {
            string completeConstraint = $"A(Of Integer).{constraint}";
            string call = $"foo.NullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A(Of Integer?).That.IsNotNull()";
            string fixedCall = $"foo.NullableIntParam({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 1);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void MakeNotNullConstraint_CodeFix_should_replace_constraint_with_AThatIsNotNull_for_indexer(string constraint)
        {
            string completeConstraint = $"A(Of Integer).{constraint}";
            string call = $"foo.Item({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A(Of Integer?).That.IsNotNull()";
            string fixedCall = $"foo.Item({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 1);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void ChangeConstraintType_CodeFix_should_replace_constraint_with_proper_type(string constraint)
        {
            string completeConstraint = $"A(Of Integer).{constraint}";
            string call = $"foo.NonNullableDoubleParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = $"A(Of Double).{constraint}";
            string fixedCall = $"foo.NonNullableDoubleParam({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 0);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void ChangeConstraintType_CodeFix_should_replace_constraint_with_proper_type_for_indexer(string constraint)
        {
            string completeConstraint = $"A(Of Short).{constraint}";
            string call = $"foo.Item({completeConstraint}, \"hello\")";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = $"A(Of Double).{constraint}";
            string fixedCall = $"foo.Item({fixedConstraint}, \"hello\")";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyVisualBasicFix(code, fixedCode, codeFixIndex: 0);
        }

        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer()
        {
            return new ArgumentConstraintTypeMismatchAnalyzer();
        }

        protected override CodeFixProvider GetVisualBasicCodeFixProvider()
        {
            return new ArgumentConstraintTypeMismatchCodeFixProvider();
        }
    }
}
