Imports NUnit.Framework
Imports FakeItEasy.Tests
Imports FakeItEasy.VisualBasic

<TestFixture()> _
Public Class ThisCallTests

    <Test(), ExpectedException(GetType(ExpectationException))> _
    Public Overridable Sub AssertWasCalled_should_fail_when_call_has_not_been_made()

        Dim foo = A.Fake(Of IFoo)()

        ThisCall.To(foo).AssertWasCalled(Function(repeat) repeat > 0) : foo.Bar()
    End Sub

    <Test()> _
    Public Overridable Sub AssertWasCalled_should_succeed_when_call_has_been_made()
        Dim foo = A.Fake(Of IFoo)()

        foo.Bar()

        ThisCall.To(foo).AssertWasCalled(Function(repeat) repeat > 0) : foo.Bar()
    End Sub

    <Test(), ExpectedException(GetType(ExpectationException))> _
    Public Overridable Sub AssertWasCalled_with_arguments_specified_should_fail_if_not_argument_predicate_passes()
        Dim foo = A.Fake(Of IFoo)()

        foo.Bar("something", "")

        ThisCall.To(foo).WhenArgumentsMatch(Function(a) a.Get(Of String)(0) = "something else").AssertWasCalled(Function(repeat) repeat = 1) : foo.Bar(Nothing, Nothing)
    End Sub

    <Test()> _
    Public Sub AssertWasCalled_should_succeed_when_arguments_matches_argument_predicate()
        Dim foo = A.Fake(Of IFoo)()

        foo.Bar("something", "")

        ThisCall.To(foo).WhenArgumentsMatch(Function(a) a.Get(Of String)(0) = "something").AssertWasCalled(Function(repeat) repeat > 0) : foo.Bar(Nothing, Nothing)
    End Sub

End Class
