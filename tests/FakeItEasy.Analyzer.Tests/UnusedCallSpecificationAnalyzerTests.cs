namespace FakeItEasy.Analyzer.Tests
{
    using FakeItEasy.Analyzer.Tests.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class UnusedCallSpecificationAnalyzerTests : DiagnosticVerifier
    {
        [Test]
        public void Diagnostic_Should_Not_Be_Triggered_When_Call_Is_Configured_With_Returns()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(() => foo.Bar()).Returns(42);
        }
    }

    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void Diagnostic_Should_Not_Be_Triggered_When_Call_Is_Asserted_With_MustHaveHappened()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(() => foo.Bar()).MustHaveHappened();
        }
    }

    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void Diagnostic_Should_Not_Be_Triggered_When_Call_Specification_Is_Assigned()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            var callToBar = A.CallTo(() => foo.Bar());
        }
    }

    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(test);
        }

        [Test]
        [SetUICulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Be_Triggered_When_Call_Specification_Is_Not_Used()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(() => foo.Bar());
        }
    }

    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = UnusedCallSpecificationAnalyzer.DiagnosticId,
                    Message =
                        "Unused call specification for 'foo.Bar()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UnusedCallSpecificationAnalyzer();
        }
    }
}
