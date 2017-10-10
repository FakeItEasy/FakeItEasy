namespace FakeItEasy.Analyzer.VisualBasic.Tests
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
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(Function() foo.Bar()).Returns(42)
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_When_Call_Is_Asserted_With_MustHaveHappened()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(Function() foo.Bar()).MustHaveHappened()
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_When_Call_Specification_Is_Assigned()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            Dim callToBar = A.CallTo(Function() foo.Bar())
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_When_Call_Specification_Is_Returned()
        {
            var test = @"Imports FakeItEasy
Imports FakeItEasy.Configuration
Namespace TheNamespace
    Class TheClass
        Function Test() As IReturnValueArgumentValidationConfiguration(Of Integer)
            Dim foo = A.Fake(Of IFoo)()
            Return A.CallTo(Function() foo.Bar())
        End Function
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(test);
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Be_Triggered_When_Call_Specification_Is_Not_Used()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(Function() foo.Bar())
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(Function() foo.Bar())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Be_Triggered_When_Call_Specification_Made_In_Global_Scope_Is_Not_Used()
        {
            var test = @"Imports FakeItEasy.A
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = Fake(Of IFoo)()
            CallTo(Function() foo.Bar())
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'CallTo(Function() foo.Bar())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_For_Call_To_With_No_Expression()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(foo)
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallsTo()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim fake = New Fake(Of IFoo)()
            fake.CallsTo(Function(x) x.Bar())
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'fake.CallsTo(Function(x) x.Bar())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_AnyCall()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim fake = New Fake(Of IFoo)()
            fake.AnyCall()
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'fake.AnyCall()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_WithAnyArguments()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(Function() foo.Bar()).WithAnyArguments()
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(Function() foo.Bar()).WithAnyArguments()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_WithReturnType_With_No_Return_Specified()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = New Fake(Of IFoo)()
            A.CallTo(foo).WithReturnType(Of Integer)()
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo).WithReturnType(Of Integer)()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_Where()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(foo).WithReturnType(Of Integer)().Where(Function(theCall) True)
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo).WithReturnType(Of Integer)().Where(Function(theCall) True)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WhereWith", Justification = "Refers to the two words 'where with'")]
        [Fact]
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_Where_With_No_Return_Type()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = New Fake(Of IFoo)()
            A.CallTo(foo).Where(Function() True, Nothing)
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo).Where(Function() True, Nothing)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_WhenArgumentsMatch_With_No_Return_Specified()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = New Fake(Of IFoo)()
            A.CallTo(foo).WhenArgumentsMatch(Function(x) True)
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo).WhenArgumentsMatch(Function(x) True)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallToSet()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallToSet(Function() foo.Bar)
        End Sub
    End Class

    Interface IFoo
        Property Bar As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallToSet(Function() foo.Bar)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallsToSet()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim fake = New Fake(Of IFoo)()
            fake.CallsToSet(Function(x) x.Bar())
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'fake.CallsToSet(Function(x) x.Bar())'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallToSet_To_Value()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallToSet(Function() foo.Bar).To(9)
        End Sub
    End Class

    Interface IFoo
        Property Bar As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallToSet(Function() foo.Bar).To(9)'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_CallToSet_To_Expression()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallToSet(Function() foo.Bar).To(Function() A(Of Integer).That.Matches(Function(i) i > 3))
        End Sub
    End Class

    Interface IFoo
        Property Bar As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallToSet(Function() foo.Bar).To(Function() A(Of Integer).That.Matches(Function(i) i > 3))'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        [Fact]
        [UsingCulture("en-US")] // so that the message is in the expected language regardless of the OS language
        public void Diagnostic_Should_Have_The_Correct_Call_Description_If_Triggered_On_WithNonVoidReturnType()
        {
            var test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(foo).WithNonVoidReturnType()
        End Sub
    End Class

    Interface IFoo
        Function Bar() As Integer
    End Interface
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0001",
                    Message =
                        "Unused call specification 'A.CallTo(foo).WithNonVoidReturnType()'; did you forget to configure or assert the call?",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 13) }
                });
        }

        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer()
        {
            return new UnusedReturnValueAnalyzer();
        }
    }
}
