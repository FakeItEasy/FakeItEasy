namespace FakeItEasy.Analyzer.VisualBasic.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Analyzer.Tests.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;

    public class NonOverridableSetupAnalyzerTests : DiagnosticVerifier
    {
        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Interface()
        {
            const string Test = @"Imports FakeItEasy
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

            this.VerifyVisualBasicDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Overridable_Member()
        {
            const string Test = @"Imports System
Namespace FakeItEasy.Analyzer.VisualBasic.Tests
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of Foo)()
            A.CallTo(Function() foo.Bar()).Returns(42)
        End Sub
    End Class

    Public Class Foo
        Public Overridable Function Bar() As Integer
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Unfortunately_Named_Call()
        {
            // Ensure only FakeItEasy triggers the diagnostic
            const string Test = @"Imports System
Namespace AnalyzerPrototypeSubjectConfusion
    Class UnfortunatelyNamedClass
        Sub TheTest()
            Dim foo = New Foo()
            A.CallTo(Sub() foo.Bar(String.Empty))
        End Sub

        Class A
            Friend Shared Sub CallTo(ByVal fake As Action)
                Throw New NotImplementedException()
            End Sub
        End Class
    End Class

    Public Class Foo
        Public Sub Bar(ByVal name As String)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Nested_Non_Overridable_Member()
        {
            const string Test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim aClass = New AClass()
            Dim foo = A.Fake(Of IFoo)()
            A.CallTo(Function() foo.Bar(aClass.Method())).Returns(42)
        End Sub
    End Class

    Class AClass
        Public Function Method() As Integer
            Return 3
        End Function
    End Class
    Interface IFoo
        Function Bar(ByVal i As Integer) As Integer
    End Interface
End Namespace";

            this.VerifyVisualBasicDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Overridable_Member()
        {
            const string Test = @"Imports System
Namespace FakeItEasy.Analyzer.VisualBasic.Tests
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of Foo)()
            A.CallTo(Function() foo.Bar()).Returns(42)
        End Sub
    End Class

    Public Class Foo
        Public Function Bar() As Integer
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 33) }
                });
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Overridable_Static_Member()
        {
            const string Test = @"Imports System
Namespace FakeItEasy.Analyzer.VisualBasic.Tests
    Class TheClass
        Sub Test()
            A.CallTo(Function() Foo.Bar()).Returns(42)
        End Sub
    End Class

    Public Class Foo
        Public Shared Function Bar() As Integer
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 5, 33) }
                });
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Overridable_Member_When_Referenced_Using_Static()
        {
            const string Test = @"Imports System
Imports FakeItEasy.A

Namespace AnalyzerPrototypeSubjectStatic
    Class TheStaticClass
        Sub TheTest()
            Dim foo = Fake(Of Foo)()
            CallTo(Sub() foo.Bar())
        End Sub
    End Class

    Friend Class Foo
        Friend Sub Bar()
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 8, 26) }
                });
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Overridable_Member_WithArguments()
        {
            const string Test = @"Imports System
Namespace FakeItEasy.Analyzer.VisualBasic.Tests
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of Foo)()
            A.CallTo(Function() foo.Bar(A(Of String).Ignored)).Returns(42)
        End Sub
    End Class

    Public Class Foo
        Public Function Bar(ByVal name As String) As Integer
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 33) }
                });
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetOn", Justification = "Refers to the two words 'set on'")]
        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Property_Set_On_Interface()
        {
            const string Test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of IFoo)()
            A.CallToSet(Function() foo.Bar)
        End Sub
    End Class

    Interface IFoo
        Property Bar() As Integer
    End Interface
End Namespace
";
            this.VerifyVisualBasicDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Indexer_On_Interface()
        {
            const string Test = @"Imports FakeItEasy
Namespace TheNamespace
    Interface IHaveAnIndexer
        Default Property Item(ByVal index As Integer) As Byte
    End Interface

    Class TheClass
        Sub Test()
            Dim fake = A.Fake(Of IHaveAnIndexer)()
            A.CallTo(Function() fake(A(Of Integer).Ignored)).MustNotHaveHappened()
        End Sub
    End Class
End Namespace
";
            this.VerifyVisualBasicDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Overridable_Indexer()
        {
            const string Test = @"Imports FakeItEasy
Namespace TheNamespace
    Class Foo
        Public Default ReadOnly Overridable Property Item(ByVal index As Integer) As Byte
            Get
                Return 0
            End Get
        End Property
    End Class

    Class TheClass
        Sub Test()
            Dim fake = A.Fake(Of Foo)()
            A.CallTo(Function() fake(A(Of Integer).Ignored)).MustNotHaveHappened()
        End Sub
    End Class
End Namespace
";
            this.VerifyVisualBasicDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Overridable_Indexer()
        {
            const string Test = @"Imports FakeItEasy
Namespace TheNamespace
    Class Foo
        Public Default ReadOnly Property Item(ByVal index As Integer) As Byte
            Get
                Return 0
            End Get
        End Property
    End Class

    Class TheClass
        Sub Test()
            Dim fake = A.Fake(Of Foo)()
            A.CallTo(Function() fake(A(Of Integer).Ignored)).MustNotHaveHappened()
        End Sub
    End Class
End Namespace
";
            this.VerifyVisualBasicDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 14, 33) }
                });
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_MustOverride_Method()
        {
            const string Test = @"Namespace FakeItEasy.Analyzer.VisualBasic.Tests
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of Foo)()
            A.CallTo(Function() foo.Bar()).Returns(42)
        End Sub
    End Class

    Public MustInherit Class Foo
        Public MustOverride Function Bar() As Integer
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Not_Be_Triggered_For_Non_Sealed_Override_Method()
        {
            const string Test = @"Namespace FakeItEasy.Analyzer.VisualBasic.Tests
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of Foo2)()
            A.CallTo(Function() foo.Bar()).Returns(42)
        End Sub
    End Class

    Public Class Foo
        Public Overridable Function Bar() As Integer
            Return 0
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(Test);
        }

        [Fact]
        public void Diagnostic_Should_Be_Triggered_For_Non_Overridable_Property_Set()
        {
            const string Test = @"Imports FakeItEasy
Namespace TheNamespace
    Class TheClass
        Sub Test()
            Dim foo = A.Fake(Of Foo)()
            A.CallToSet(Function() foo.Bar)
        End Sub
    End Class

    Class Foo
        Property Bar() As Integer
    End Class
End Namespace
";

            this.VerifyVisualBasicDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 36) }
                });
        }

        [Fact]
        public void Diagnostic_Should_Correctly_Reflect_Member_Name()
        {
            // Test to ensure member name hasn't mistakenly been hardcoded
            const string Test = @"Imports System
Namespace FakeItEasy.Analyzer.VisualBasic.Tests
    Class TheClass
        Sub TheTest()
            Dim foo = A.Fake(Of Foo)()
            A.CallTo(Sub() foo.DifferentNameThanOtherTests(A(Of String).Ignored))
        End Sub
    End Class

    Public Class Foo
        public Sub DifferentNameThanOtherTests(ByVal name As String)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace";

            this.VerifyVisualBasicDiagnostic(
                Test,
                new DiagnosticResult
                {
                    Id = "FakeItEasy0002",
                    Message =
                        "Member DifferentNameThanOtherTests can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.vb", 6, 28) }
                });
        }

        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }
    }
}
