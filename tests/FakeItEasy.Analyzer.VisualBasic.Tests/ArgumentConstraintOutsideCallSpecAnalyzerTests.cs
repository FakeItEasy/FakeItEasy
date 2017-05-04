namespace FakeItEasy.Analyzer.VisualBasic.Tests
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
                "A(Of Integer).Ignored",
                "A(Of Integer).That.IsEqualTo(42)",
                "A(Of Integer).That.Not.IsEqualTo(42)");

        [Theory]
        [InlineData("A(Of Integer).Ignored")]
        [InlineData("A(Of Integer).That")]
        [InlineData("A(Of Integer).That.Not")]
        [InlineData("A(Of Integer).That.IsEqualTo(42)")]
        [InlineData("A(Of Integer).That.Not.IsEqualTo(42)")]
        public void Diagnostic_should_be_triggered_for_constraint_assigned_to_variable(string constraint)
        {
            string code = $@"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim x = {constraint}
        End Sub
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0003",
                    Message = $"Argument constraint '{constraint}' is not valid outside a call specification.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 5, 21) }
                });
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_be_triggered_in_A_CallToSet_To_Value(string constraint)
        {
            string code = $@"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallToSet(Function() foo.Bar).To({constraint}).DoesNothing()
        End Sub
    End Class

    Interface IFoo
        Property Bar As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                code,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0003",
                    Message = $"Argument constraint '{constraint}' is not valid outside a call specification.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 48) }
                });
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_A_CallTo_Func(string constraint)
        {
            string code = $@"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(Function() foo.Bar({constraint})).Returns(42)
        End Sub
    End Class

    Interface IFoo
        Function Bar(ByVal x As Integer) As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_A_CallTo_Action(string constraint)
        {
            string code = $@"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(Sub() foo.Bar({constraint})).DoesNothing()
        End Sub
    End Class

    Interface IFoo
        Sub Bar(ByVal x As Integer)
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_A_CallToSet_To_Expression(string constraint)
        {
            string code = $@"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallToSet(Function() foo.Bar).To(Function() {constraint}).DoesNothing()
        End Sub
    End Class

    Interface IFoo
        Property Bar As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_Fake_CallsTo_Func(string constraint)
        {
            string code = $@"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim fake = new Fake(Of IFoo)()
            fake.CallsTo(Function(foo) foo.Bar({constraint})).Returns(42)
        End Sub
    End Class

    Interface IFoo
        Function Bar(ByVal x As Integer) As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_Fake_CallTo_Action(string constraint)
        {
            string code = $@"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim fake = new Fake(Of IFoo)()
            fake.CallsTo(Sub(foo) foo.Bar({constraint})).DoesNothing()
        End Sub
    End Class

    Interface IFoo
        Sub Bar(ByVal x As Integer)
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(Constraints))]
        public void Diagnostic_should_not_be_triggered_in_Fake_CallsToSet_To_Expression(string constraint)
        {
            string code = $@"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim fake = new Fake(Of IFoo)()
            fake.CallsToSet(Function(foo) foo.Bar).To(Function() {constraint}).DoesNothing()
        End Sub
    End Class

    Interface IFoo
        Property Bar As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer() =>
            new ArgumentConstraintOutsideCallSpecAnalyzer();
    }
}
