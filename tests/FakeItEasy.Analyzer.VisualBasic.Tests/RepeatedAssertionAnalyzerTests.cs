namespace FakeItEasy.Analyzer.VisualBasic.Tests
{
    using System.Collections.Generic;
    using FakeItEasy.Analyzer.Tests.Helpers;
    using FakeItEasy.Tests;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    public class RepeatedAssertionAnalyzerTests : DiagnosticVerifier
    {
        private const string CodeTemplate = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim fake = A.Fake(Of IFoo)()
            A.CallTo(Sub() fake.Method()).{0}
        End Sub
    End Class

    Interface IFoo
        Function Method() As Integer
    End Interface
End Namespace";

        [Theory]
        [MemberData(nameof(AssertionsThatDoNotUseRepeated))]
        public void Diagnostic_Should_Not_Be_Triggered_For_Assertion_That_Does_Not_Use_Repeated(string assertion)
        {
            var code = string.Format(CodeTemplate, assertion);
            this.VerifyVisualBasicDiagnostic(code);
        }

        [Theory]
        [MemberData(nameof(AssertionsThatUseRepeated))]
        public void Diagnostic_Should_Be_Triggered_For_MustHaveHappened_With_Repeated_Argument(string assertion)
        {
            var code = string.Format(CodeTemplate, assertion);
            this.VerifyVisualBasicDiagnostic(
                 code,
                 new DiagnosticResult
                 {
                     Id = "FakeItEasy0006",
                     Message = "The MustHaveHappened(Repeated) assertion is being retired, and will be deprecated in FakeItEasy version 5.0.0 and removed in version 6.0.0.",
                     Severity = DiagnosticSeverity.Warning,
                     Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 43) }
                 });
        }

        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer()
        {
            return new RepeatedAssertionAnalyzer();
        }

        private static IEnumerable<object[]> AssertionsThatDoNotUseRepeated() =>
            TestCases.FromObject(
                "MustHaveHappened()",
                "MustNotHaveHappened()",
                "MustHaveHappenedOnceExactly()",
                "MustHaveHappenedOnceOrMore()",
                "MustHaveHappenedOnceOrLess()",
                "MustHaveHappenedTwiceExactly()",
                "MustHaveHappenedTwiceOrMore()",
                "MustHaveHappenedTwiceOrLess()",
                "MustHaveHappened(3, Times.Exactly)",
                "MustHaveHappened(3, Times.OrMore)",
                "MustHaveHappened(3, Times.OrLess)",
                "MustHaveHappenedANumberOfTimesMatching(Function(n) n Mod 2 = 0)");

        private static IEnumerable<object[]> AssertionsThatUseRepeated() =>
            TestCases.FromObject(
                "MustHaveHappened(Repeated.Never)",
                "MustHaveHappened(Repeated.Exactly.Once)",
                "MustHaveHappened(Repeated.AtLeast.Twice)",
                "MustHaveHappened(Repeated.NoMoreThan.Times(3))",
                "MustHaveHappened(Repeated.Like(Function(n) n < 3 Or n > 19))");
    }
}
