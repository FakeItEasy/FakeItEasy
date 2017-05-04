namespace FakeItEasy.Analyzer.CSharp.Tests
{
    using System.Collections.Generic;
    using FakeItEasy.Analyzer.Tests.Helpers;
    using FakeItEasy.Tests;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    public class ArgumentConstraintOutsideCallSpecAnalyzerTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> Constraints =>
            TestCases.FromObject(
                "A<int>._",
                "A<int>.Ignored",
                "A<int>.That.IsEqualTo(42)",
                "A<int>.That.Not.IsEqualTo(42)");

        [Theory]
        [InlineData("A<int>._")]
        [InlineData("A<int>.Ignored")]
        [InlineData("A<int>.That")]
        [InlineData("A<int>.That.Not")]
        [InlineData("A<int>.That.IsEqualTo(42)")]
        [InlineData("A<int>.That.Not.IsEqualTo(42)")]
        public void Diagnostic_should_be_triggered_for_constraint_assigned_to_variable(string constraint)
        {
            string code = $@"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void Test()
        {{
            var x = {constraint};
        }}
    }}
}}
";

            this.VerifyCSharpDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0003",
                    Message = $"Argument constraint '{constraint}' is not valid outside a call specification.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 21) }
                });
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_be_triggered_in_A_CallToSet_To_Value(string constraint)
        {
            string code = $@"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void Test()
        {{
            var foo = A.Fake<IFoo>();
            A.CallToSet(() => foo.Bar).To({constraint}).DoesNothing();
        }}
    }}

    interface IFoo {{ int Bar {{ get; set; }} }}
}}
";

            this.VerifyCSharpDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0003",
                    Message = $"Argument constraint '{constraint}' is not valid outside a call specification.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 43) }
                });
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_A_CallTo_Func(string constraint)
        {
            string code = $@"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void Test()
        {{
            var foo = A.Fake<IFoo>();
            A.CallTo(() => foo.Bar({constraint})).Returns(42);
        }}
    }}

    interface IFoo {{ int Bar(int x); }}
}}
";

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_A_CallTo_Action(string constraint)
        {
            string code = $@"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void Test()
        {{
            var foo = A.Fake<IFoo>();
            A.CallTo(() => foo.Bar({constraint})).DoesNothing();
        }}
    }}

    interface IFoo {{ void Bar(int x); }}
}}
";

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_A_CallToSet_To_Expression(string constraint)
        {
            string code = $@"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void Test()
        {{
            var foo = A.Fake<IFoo>();
            A.CallToSet(() => foo.Bar).To(() => {constraint}).DoesNothing();
        }}
    }}

    interface IFoo {{ int Bar {{ get; set; }} }}
}}
";

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_Fake_CallsTo_Func(string constraint)
        {
            string code = $@"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void Test()
        {{
            var fake = new Fake<IFoo>();
            fake.CallsTo(foo => foo.Bar({constraint})).Returns(42);
        }}
    }}

    interface IFoo {{ int Bar(int x); }}
}}
";

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_Fake_CallTo_Action(string constraint)
        {
            string code = $@"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void Test()
        {{
            var fake = new Fake<IFoo>();
            fake.CallsTo(foo => foo.Bar({constraint})).DoesNothing();
        }}
    }}

    interface IFoo {{ void Bar(int x); }}
}}
";

            this.VerifyCSharpDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_Fake_CallsToSet_To_Expression(string constraint)
        {
            string code = $@"using FakeItEasy;
namespace TheNamespace
{{
    class TheClass
    {{
        void Test()
        {{
            var fake = new Fake<IFoo>();
            fake.CallsToSet(foo => foo.Bar).To(() => {constraint}).DoesNothing();
        }}
    }}

    interface IFoo {{ int Bar {{ get; set; }} }}
}}
";

            this.VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new ArgumentConstraintOutsideCallSpecAnalyzer();
    }
}
