using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FakeItEasy.Analyzer.Tests
{
    using Helpers;
    using Xunit;

    public class NonVirtualSetupAnalyzerTests:DiagnosticVerifier
    {
        //The aim to create an analyzer that picks up
        //on any call setups that target a non-virtual member

        //A.CallTo(() => shop.GetTopSellingCandy()).Returns(lollipop); //Warning here

        //Tests to implement:
        //Virtual Member
        //Non-virtual member
        //Interface
        //Non-virtual member with arguments
        //Non-virtual member with returns

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Interface()
        {
            const string test = @"using FakeItEasy;
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

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Member()
        {
            var test = @"using System;

//Temporary class to get non-virtual test correct
namespace FakeItEasy.Analyzer.Tests
{
    class TheClass
    {
        void TheTest()
        {
            var foo = A.Fake<Foo>();
            A.CallTo(() => foo.Bar());
        }
    }

    internal class Foo
    {
        internal void Bar()
        {
            throw new NotImplementedException();
        }
    }
}

";

            VerifyCSharpDiagnostic(
    test,
    new DiagnosticResult
    {
        Id = DiagnosticDefinitions.UnusedCallSpecification.Id,
        Message =
            "Non virtual methods can not be intercepted",
        Severity = DiagnosticSeverity.Warning,
        Locations = new[] { new DiagnosticResultLocation("Test0.cs", 28, 28) }
    });

        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }

    }

}
