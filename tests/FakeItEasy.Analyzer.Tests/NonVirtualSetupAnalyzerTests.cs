namespace FakeItEasy.Analyzer.Tests
{
    using FakeItEasy.Analyzer.Tests.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    public class NonVirtualSetupAnalyzerTests:DiagnosticVerifier
    {
        //The aim to create an analyzer that picks up
        //on any call setups that target a non-virtual member

        //A.CallTo(() => shop.GetTopSellingCandy()).Returns(lollipop); //Warning here

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
        public void Diagnostic_Should_Not_Be_Triggered_For_Virtual_Member()
        {
            const string test = @"using System;

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
        internal virtual void Bar()
        {
            throw new NotImplementedException();
        }
    }
}

";
            VerifyCSharpDiagnostic(test);

        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Unfortunately_Named_Call()
        {
            //Ensure only FakeItEasy triggers the diagnostic

            const string test = @"using System;
namespace AnalyzerPrototypeSubjectConfusion
{
    class UnfortunatelyNamedClass
    {
        void TheTest()
        {
            var foo = new Foo();
            A.CallTo(() => foo.Bar(string.Empty));
        }

        static class A
        {
            internal static void CallTo(Action fake)
            {
                throw new NotImplementedException();
            }
        }
    }

    internal class Foo
    {
        internal void Bar(string name)
        {
            throw new NotImplementedException();
        }
    }

}";

            VerifyCSharpDiagnostic(test);

        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Nested_Non_Virtual_Member()
        {
            const string test = @"namespace TheNamespace
{
    using FakeItEasy;
    class TheClass
    {
        void Test()
        {
            var aClass = new AClass();
            var foo = A.Fake<IFoo>();
            A.CallTo(() => foo.Bar(aClass.Method())).Returns(42);
        }
    }

    class AClass { public int Method() { return 3; } }
    interface IFoo { int Bar(int i); }
}";

            VerifyCSharpDiagnostic(test);

        }


        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Member()
        {
            const string test = @"using System;

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
        Id = DiagnosticDefinitions.NonVirtualSetup.Id,
        Message =
            "Non virtual member 'Bar' cannot be intercepted.",
        Severity = DiagnosticSeverity.Warning,
        Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 28) }
    });

        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Member_WithArguments()
        {
            const string test = @"using System;

//Temporary class to get non-virtual test correct
    namespace FakeItEasy.Analyzer.Tests
    {
    class TheClass
    {
        void TheTest()
        {
            var foo = A.Fake<Foo>();
            A.CallTo(() => foo.Bar(A<string>.Ignored));
        }
    }

    internal class Foo
    {
        internal void Bar(string name)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpDiagnostic(
        test,
        new DiagnosticResult
        {
            Id = DiagnosticDefinitions.NonVirtualSetup.Id,
            Message =
             "Non virtual member 'Bar' cannot be intercepted.",
            Severity = DiagnosticSeverity.Warning,
            Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 28) }
        });


        }

        [Fact]
        public void Diagnostic_Should_Correctly_Reflect_Member_Name()
        {
            //Test to ensure member name hasn't mistakenly been hardcoded

            const string test = @"using System;

//Temporary class to get non-virtual test correct
    namespace FakeItEasy.Analyzer.Tests
    {
    class TheClass
    {
        void TheTest()
        {
            var foo = A.Fake<Foo>();
            A.CallTo(() => foo.DifferentNameThanOtherTests(A<string>.Ignored));
        }
    }

    internal class Foo
    {
        internal void DifferentNameThanOtherTests(string name)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpDiagnostic(
        test,
        new DiagnosticResult
        {
            Id = DiagnosticDefinitions.NonVirtualSetup.Id,
            Message =
             "Non virtual member 'DifferentNameThanOtherTests' cannot be intercepted.",
            Severity = DiagnosticSeverity.Warning,
            Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 28) }
        });


        }


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }

    }

}
