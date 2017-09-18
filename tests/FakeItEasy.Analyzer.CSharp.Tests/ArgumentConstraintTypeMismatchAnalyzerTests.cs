namespace FakeItEasy.Analyzer.CSharp.Tests
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
        private const string CodeTemplate = @"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void TheTest()
        {{
            var foo = A.Fake<IFoo>();
            A.CallTo(() => {0}).Returns(42);
        }}
    }}

    interface IFoo
    {{
        int NullableIntParam(int? x);
        int NullableLongParam(long? x);
        int NonNullableIntParam(int x);
        int NonNullableDoubleParam(double x);
        int BaseClassParam(BaseClass x);
        int HasImplicitConversionParam(CanBeConvertedTo x);
        int this[int? x] {{ get; }}
        int this[double x, string s] {{ get; }}
    }}

    class BaseClass
    {{
    }}

    class DerivedClass: BaseClass
    {{
    }}

    class CanBeConvertedTo
    {{
    }}

    class CanBeConvertedFrom
    {{
        public static implicit operator CanBeConvertedTo(CanBeConvertedFrom x)
        {{
            return new CanBeConvertedTo();
        }}
    }}
}}";

        public static IEnumerable<object[]> FakeItEasy0004SupportedConstraints =>
            TestCases.FromObject(
                "_",
                "Ignored");

        public static IEnumerable<object[]> FakeItEasy0005SupportedConstraints =>
            TestCases.FromObject(
                "_",
                "Ignored",
                "That.Matches(_ => true)");

        public static IEnumerable<object[]> AllSupportedConstraints => FakeItEasy0005SupportedConstraints;

        [Theory]
        [MemberData(nameof(AllSupportedConstraints))]
        public void No_diagnostic_should_be_triggered_when_nullable_constraint_is_used_for_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int?>.{constraint}";
            string call = $"foo.NullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(AllSupportedConstraints))]
        public void No_diagnostic_should_be_triggered_when_non_nullable_constraint_is_used_for_non_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo.NonNullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(AllSupportedConstraints))]
        public void No_diagnostic_should_be_triggered_when_derived_nonnullable_class_constraint_is_used_with_base_class_parameter(string constraint)
        {
            string completeConstraint = $"A<DerivedClass>.{constraint}";
            string call = $"foo.BaseClassParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void FakeItEasy0004_should_be_triggered_when_non_nullable_constraint_is_used_for_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo.NullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0004",
                    Message = $"Parameter 'x' of method or indexer 'NullableIntParam' is nullable, but argument constraint '{completeConstraint}' is not. Calls where this argument is null will not be matched.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 49) }
                });
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void FakeItEasy0004_should_be_triggered_when_non_nullable_constraint_is_used_for_nullable_parameter_for_indexer(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo[{completeConstraint}]";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0004",
                    Message = $"Parameter 'x' of method or indexer 'this[]' is nullable, but argument constraint '{completeConstraint}' is not. Calls where this argument is null will not be matched.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 32) }
                });
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void FakeItEasy0005_should_be_triggered_when_wrong_typed_nonnullable_constraint_is_used_with_nonnullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo.NonNullableDoubleParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0005",
                    Message = $"Parameter 'x' of method or indexer 'NonNullableDoubleParam' has type double, but argument constraint '{completeConstraint}' has type int. No calls can match this constraint.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 55) }
                });
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void FakeItEasy0005_should_be_triggered_when_wrong_typed_nullable_constraint_is_used_with_nullable_parameter(string constraint)
        {
            string completeConstraint = $"A<int?>.{constraint}";
            string call = $"foo.NullableLongParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0005",
                    Message = $"Parameter 'x' of method or indexer 'NullableLongParam' has type long?, but argument constraint '{completeConstraint}' has type int?. No calls can match this constraint.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 50) }
                });
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void FakeItEasy0005_should_be_triggered_when_constraint_with_implicit_conversion_is_used_with_target_type(string constraint)
        {
            string completeConstraint = $"A<CanBeConvertedFrom>.{constraint}";
            string call = $"foo.HasImplicitConversionParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            this.VerifyCSharpDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0005",
                    Message = $"Parameter 'x' of method or indexer 'HasImplicitConversionParam' has type TheNamespace.CanBeConvertedTo, but argument constraint '{completeConstraint}' has type TheNamespace.CanBeConvertedFrom. No calls can match this constraint.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 59) }
                });
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void MakeConstraintNullable_CodeFix_should_replace_constraint_with_nullable_constraint(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo.NullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = $"A<int?>.{constraint}";
            string fixedCall = $"foo.NullableIntParam({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyCSharpFix(code, fixedCode, codeFixIndex: 0);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void MakeConstraintNullable_CodeFix_should_replace_constraint_with_nullable_constraint_for_indexer(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo[{completeConstraint}]";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = $"A<int?>.{constraint}";
            string fixedCall = $"foo[{fixedConstraint}]";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyCSharpFix(code, fixedCode, codeFixIndex: 0);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void MakeNotNullConstraint_CodeFix_should_replace_constraint_with_AThatIsNotNull(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo.NullableIntParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A<int?>.That.IsNotNull()";
            string fixedCall = $"foo.NullableIntParam({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyCSharpFix(code, fixedCode, codeFixIndex: 1);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0004SupportedConstraints))]
        public void MakeNotNullConstraint_CodeFix_should_replace_constraint_with_AThatIsNotNull_for_indexer(string constraint)
        {
            string completeConstraint = $"A<int>.{constraint}";
            string call = $"foo[{completeConstraint}]";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = "A<int?>.That.IsNotNull()";
            string fixedCall = $"foo[{fixedConstraint}]";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyCSharpFix(code, fixedCode, codeFixIndex: 1);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void ChangeConstraintType_CodeFix_should_replace_constraint_with_proper_type(string constraint)
        {
            string completeConstraint = $"A<short>.{constraint}";
            string call = $"foo.NonNullableDoubleParam({completeConstraint})";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = $"A<double>.{constraint}";
            string fixedCall = $"foo.NonNullableDoubleParam({fixedConstraint})";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyCSharpFix(code, fixedCode, codeFixIndex: 0);
        }

        [Theory]
        [MemberData(nameof(FakeItEasy0005SupportedConstraints))]
        public void ChangeConstraintType_CodeFix_should_replace_constraint_with_proper_type_for_indexer(string constraint)
        {
            string completeConstraint = $"A<short>.{constraint}";
            string call = $"foo[{completeConstraint}, \"hello\"]";
            string code = string.Format(CodeTemplate, call);

            string fixedConstraint = $"A<double>.{constraint}";
            string fixedCall = $"foo[{fixedConstraint}, \"hello\"]";
            string fixedCode = string.Format(CodeTemplate, fixedCall);
            this.VerifyCSharpFix(code, fixedCode, codeFixIndex: 0);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ArgumentConstraintTypeMismatchAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ArgumentConstraintTypeMismatchCodeFixProvider();
        }
    }
}
