Imports System.Runtime.InteropServices
Imports FluentAssertions
Imports Xunit

Public Class RefArgumentConstraintTests

    Public Interface IFoo
        Function Bar(ByRef refParameter As String) As Integer
        Function Baz(<Out> ByRef outParameter As String) As Integer
    End Interface

    <Fact>
    Public Sub Constraint_on_ref_argument_is_honored()
        ' Arrange
        Dim foo = A.Fake(Of IFoo)
        A.CallTo(Function() foo.Bar(A(Of String).That.StartsWith("h"))).Returns(42)

        ' Act
        Dim result = foo.Bar("hello")

        ' Assert
        result.Should().Be(42)
    End Sub

    <Fact>
    Public Sub Constraint_on_out_argument_is_ignored()
        ' Arrange
        Dim foo = A.Fake(Of IFoo)
        A.CallTo(Function() foo.Baz(A(Of String).That.StartsWith("h"))).Returns(42)

        ' Act
        Dim result = foo.Baz("blah")

        ' Assert
        result.Should().Be(42)
    End Sub

End Class
