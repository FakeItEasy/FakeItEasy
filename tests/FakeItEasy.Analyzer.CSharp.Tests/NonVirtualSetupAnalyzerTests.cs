namespace FakeItEasy.Analyzer.CSharp.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Analyzer.Tests.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    public class NonVirtualSetupAnalyzerTests : DiagnosticVerifier
    {
        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Interface()
        {
            const string Test = @"using FakeItEasy;
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

            this.VerifyCSharpDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Virtual_Member()
        {
            const string Test = @"using System;

//Temporary class to get non-virtual Test correct
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
            this.VerifyCSharpDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Unfortunately_Named_Call()
        {
            // Ensure only FakeItEasy triggers the diagnostic
            const string Test = @"using System;
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

            this.VerifyCSharpDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Nested_Non_Virtual_Member()
        {
            const string Test = @"namespace TheNamespace
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

            this.VerifyCSharpDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Member()
        {
            const string Test = @"using System;

//Temporary class to get non-virtual Test correct
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

            this.VerifyCSharpDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 28) }
                });
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Static_Member()
        {
            const string Test = @"using System;

//Temporary class to get non-virtual Test correct
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

            this.VerifyCSharpDiagnostic(
     Test,
     new DiagnosticResult
     {
         Id = "FakeItEasy0002",
         Message =
             "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
         Severity = DiagnosticSeverity.Error,
         Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 28) }
     });
    }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Member_When_Referenced_Using_Static()
        {
            const string Test = @"using System;
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

            this.VerifyCSharpDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 26) }
                });
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Member_WithArguments()
        {
            const string Test = @"using System;

//Temporary class to get non-virtual Test correct
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

            this.VerifyCSharpDiagnostic(
        Test,
        new DiagnosticResult
        {
            Id = "FakeItEasy0002",
            Message =
             "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            Severity = DiagnosticSeverity.Error,
            Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 28) }
        });
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetOn", Justification = "Refers to the two words 'set on'")]
        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Property_Set_On_Interface()
        {
            const string Test = @"namespace TheNamespace
{
    using FakeItEasy;

    public class TheClass
    {
        public void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallToSet(() => foo.Bar);
        }
    }

    public interface IFoo
    {
        int Bar{get;}
    }
}";
            this.VerifyCSharpDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Indexer_On_Interface()
        {
            const string Test = @"
namespace TheNamespace
{
    using FakeItEasy;

    public interface IHaveAnIndexer { string this[int context] { get; } }

    public class TheClass
    {
        public void Test()
        {
            var fake = A.Fake<IHaveAnIndexer>();
            A.CallTo(() => fake[A<int>._]).MustHaveHappened();
        }
    }
}
";

            this.VerifyCSharpDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Virtual_Indexer()
        {
            const string Test = @"
namespace TheNamespace
{
    using FakeItEasy;

    public class Foo
    {
        public virtual string this[int context] => ""hello"";
    }

    public class TheClass
    {
        public void Test()
        {
            var fake = A.Fake<Foo>();
            A.CallTo(() => fake[A<int>._]).MustHaveHappened();
        }
    }
}
";

            this.VerifyCSharpDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Indexer()
        {
            const string Test = @"
namespace TheNamespace
{
    using FakeItEasy;

    public class Foo
    {
        public string this[int context] => ""hello"";
    }

    public class TheClass
    {
        public void Test()
        {
            var fake = A.Fake<Foo>();
            A.CallTo(() => fake[A<int>._]).MustHaveHappened();
        }
    }
}
";

            this.VerifyCSharpDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 16, 28) }
                });
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Abstract_Method()
        {
            const string Test = @"
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

            this.VerifyCSharpDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Non_Sealed_Override_Method()
        {
            const string Test = @"
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

            this.VerifyCSharpDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Virtual_Property_Set()
        {
            const string Test = @"using FakeItEasy;

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

            this.VerifyCSharpDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 10, 31) }
                });
         }

        [Fact]
        public void Diagnostic_Should_Correctly_Reflect_Member_Name()
        {
            // Test to ensure member name hasn't mistakenly been hardcoded
            const string Test = @"using System;

//Temporary class to get non-virtual Test correct
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

            this.VerifyCSharpDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                     "Member DifferentNameThanOtherTests can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 28) }
                });
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }
    }
}
