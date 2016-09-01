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

    public class Foo
    {
        public virtual void Bar()
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

    public class Foo
    {
        public void Bar(string name)
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

    public class Foo
    {
        public void Bar()
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
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Static_Member()
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
            A.CallTo(() => Foo.Bar());
        }
    }

    public class Foo
    {
        public static void Bar()
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
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Member_When_Referenced_Using_Static()
        {
            const string test = @"using System;
using static FakeItEasy.A;

namespace AnalyzerPrototypeSubjectStatic
{
    class TheStaticClass
    {
        void TheTest()
        {
            var foo = Fake<Foo>();
            CallTo(() => foo.Bar());
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
        Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 26) }
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

    public class Foo
    {
        public void Bar(string name)
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
        public void Diagnostic_Should_Not_Be_Triggered_For_Property_Set_On_Interface()
        {
            const string test = @"namespace TheNamespace
{
    using FakeItEasy;

    public class TheClass
    {
        public void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(() => foo.Bar);
        }
    }

    public interface IFoo
    {
        int Bar{get;}
    }
}";
            VerifyCSharpDiagnostic(test);

        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Abstract_Method()
        {
            const string test = @"
namespace TheNamespace
{
    using FakeItEasy;

    public class TheClass
    {
        public void Test()
        {
            var foo = A.Fake<Foo>();
            A.CallTo(() => foo.Bar()).Returns(42);
        }
    }

    public abstract class Foo { public abstract int Bar(); }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Non_Sealed_Override_Method()
        {
            const string test = @"
namespace TheNamespace
{
    using FakeItEasy;

    public class TheClass
    {
        public void Test()
        {
            var foo = A.Fake<Foo2>();
            A.CallTo(() => foo.Bar()).Returns(42);
        }
    }

    public class Foo { public virtual int Bar() => 0; }
    public class Foo2 : Foo { public override int Bar() => 1; }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Property_Set()
        {
            const string test = @"using FakeItEasy;

namespace PrototypeProperty
{
    class TheClass
    {
        void TheTest()
        {
            var foo = A.Fake<Foo>();
            A.CallToSet(() => foo.Bar);
        }
    }

    internal class Foo
    {
        public int Bar { get; set; }
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
                    Locations = new[] {new DiagnosticResultLocation("Test0.cs", 10, 31)}
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

    public class Foo
    {
        public void DifferentNameThanOtherTests(string name)
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
