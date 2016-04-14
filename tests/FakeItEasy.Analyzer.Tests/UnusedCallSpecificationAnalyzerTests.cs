namespace FakeItEasy.Analyzer.Tests
{
    using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallTo", Justification = "It's an identifier")]
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
                    Id = DiagnosticDefinitions.UnusedCallSpecification.Id,
                    Message =
                        "Unused call specification 'A.CallTo(() => foo.Bar())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Test]
        [SetUICulture("en-US")] // so that the message is in the expected language regardless of the OS language
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallTo", Justification = "It's an identifier")]
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
                    Id = DiagnosticDefinitions.UnusedCallSpecification.Id,
                    Message =
                        "Unused call specification 'A.CallTo(foo)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Test]
        [SetUICulture("en-US")] // so that the message is in the expected language regardless of the OS language
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallTo", Justification = "It's an identifier")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "WithAnyArguments", Justification = "It's an identifier")]
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
                    Id = DiagnosticDefinitions.UnusedCallSpecification.Id,
                    Message =
                        "Unused call specification 'A.CallTo(() => foo.Bar()).WithAnyArguments()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Test]
        [SetUICulture("en-US")] // so that the message is in the expected language regardless of the OS language
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallTo", Justification = "It's an identifier")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "WithReturnType", Justification = "It's an identifier")]
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
                    Id = DiagnosticDefinitions.UnusedCallSpecification.Id,
                    Message =
                        "Unused call specification 'A.CallTo(foo).WithReturnType<int>().Where(call => true)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Test]
        [SetUICulture("en-US")] // so that the message is in the expected language regardless of the OS language
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallToSet", Justification = "It's an identifier")]
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
                    Id = DiagnosticDefinitions.UnusedCallSpecification.Id,
                    Message =
                        "Unused call specification 'A.CallToSet(() => foo.Bar)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Test]
        [SetUICulture("en-US")] // so that the message is in the expected language regardless of the OS language
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallToSet", Justification = "It's an identifier")]
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
                    Id = DiagnosticDefinitions.UnusedCallSpecification.Id,
                    Message =
                        "Unused call specification 'A.CallToSet(() => foo.Bar).To(9)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                });
        }

        [Test]
        [SetUICulture("en-US")] // so that the message is in the expected language regardless of the OS language
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CallToSet", Justification = "It's an identifier")]
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
            A.CallToSet(() => foo.Bar).To(() => A<int>.That.Matches(i => i > 3);
        }
    }

    interface IFoo { int Bar { get; set; } }
}
";

            this.VerifyCSharpDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = DiagnosticDefinitions.UnusedCallSpecification.Id,
                    Message =
                        "Unused call specification 'A.CallToSet(() => foo.Bar).To(() => A<int>.That.Matches(i => i > 3)'; did you forget to configure or assert the call?",
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
