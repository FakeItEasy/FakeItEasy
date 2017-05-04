namespace FakeItEasy.Analyzer.CSharp.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Analyzer.Tests.Helpers;
    using FakeItEasy.Tests.TestHelpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    public class UnusedReturnValueAnalyzerTests : DiagnosticVerifier
    {
        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_When_Call_Specification_Is_Returned()
        {
            var test = @"using FakeItEasy;
using FakeItEasy.Configuration;
namespace TheNamespace
{
    class TheClass
    {
        IReturnValueArgumentValidationConfiguration<int> Test()
        {
            var foo = A.Fake<IFoo>();
            return A.CallTo(() => foo.Bar());
        }
    }

    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(test);
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
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
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(() => foo.Bar())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Be_Triggered_When_Call_Specification_Made_In_Global_Scope_Is_Not_Used()
        {
            var test = @"using static FakeItEasy.A;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = Fake<IFoo>();
            CallTo(() => foo.Bar());
        }
    }

    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'CallTo(() => foo.Bar())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_For_Call_To_With_No_Expression()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(foo);
        }
    }
    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallsTo()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var fake = new Fake<IFoo>();
            fake.CallsTo(x => x.Bar());
        }
    }
    interface IFoo { int Bar(); }
}";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'fake.CallsTo(x => x.Bar())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_AnyCall()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var fake = new Fake<IFoo>();
            fake.AnyCall();
        }
    }
    interface IFoo { int Bar(); }
}";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'fake.AnyCall()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_WithAnyArguments()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(() => foo.Bar()).WithAnyArguments();
        }
    }
    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(() => foo.Bar()).WithAnyArguments()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_WithReturnType_With_No_Return_Specified()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(foo).WithReturnType<int>();
        }
    }
    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo).WithReturnType<int>()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_Where()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(foo).WithReturnType<int>().Where(call => true);
        }
    }
    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo).WithReturnType<int>().Where(call => true)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WhereWith", Justification = "Refers to the two words 'where with'")]
        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_Where_With_No_Return_Type()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var fake = A.Fake<IFoo>();
            A.CallTo(fake).Where(call => true, output => new object());
        }
    }
    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(fake).Where(call => true, output => new object())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_WhenArgumentsMatch_With_No_Return_Specified()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(foo).WhenArgumentsMatch(x => true);
        }
    }
    interface IFoo { int Bar(); }
}";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo).WhenArgumentsMatch(x => true)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallToSet()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallToSet(() => foo.Bar);
        }
    }

    interface IFoo { int Bar { get; set; } }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallToSet(() => foo.Bar)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallsToSet()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var fake = new Fake<IFoo>();
            fake.CallsToSet(x => x.Bar());
        }
    }
    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'fake.CallsToSet(x => x.Bar())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallToSet_To_Value()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallToSet(() => foo.Bar).To(9);
        }
    }

    interface IFoo { int Bar { get; set; } }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallToSet(() => foo.Bar).To(9)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallToSet_To_Expression()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallToSet(() => foo.Bar).To(() => A<int>.That.Matches(i => i > 3));
        }
    }

    interface IFoo { int Bar { get; set; } }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallToSet(() => foo.Bar).To(() => A<int>.That.Matches(i => i > 3))'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_WithNonVoidReturnType()
        {
            var test = @"using FakeItEasy;
namespace TheNamespace
{
    class TheClass
    {
        void Test()
        {
            var foo = A.Fake<IFoo>();
            A.CallTo(foo).WithNonVoidReturnType();
        }
    }
    interface IFoo { int Bar(); }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo).WithNonVoidReturnType()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UnusedReturnValueAnalyzer();
        }
    }
}
