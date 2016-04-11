Imports FakeItEasy
Imports FakeItEasy.Examples.ExampleObjects

Public Class AssertingCalls
    Public Sub Asserting_a_sub_using_lambda()
        Dim widget = A.Fake(Of IWidget)()

        A.CallTo(Sub() widget.Repair()).MustHaveHappened()
    End Sub

    Public Sub Asserting_a_function_using_lambda()
        Dim factory = A.Fake(Of IWidgetFactory)()

        A.CallTo(Function() factory.CreateWithColor(A(Of String).Ignored)).MustHaveHappened()
    End Sub

    Public Sub Asserting_ordered_function_calls()
        Dim factory = A.Fake(Of IWidgetFactory)()

        A.CallTo(Function() factory.CreateWithColor(A(Of String).Ignored)).MustHaveHappened() _
            .Then(A.CallTo(Function() factory.CreateWithColor("orange")).MustHaveHappened())
    End Sub
End Class
