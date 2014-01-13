Imports FakeItEasy
Imports FakeItEasy.Examples.ExampleObjects

Public Class ConfiguringCalls
    Public Sub Configuring_a_sub_to_throw_an_exception()
        Dim widget = A.Fake(Of IWidget)()

        NextCall.To(widget).Throws(New NotSupportedException()) : widget.Repair()
    End Sub

    Public Sub Configuring_a_sub_using_lambda()
        Dim widget = A.Fake(Of IWidget)()

        A.CallTo(Sub() widget.Repair()).DoesNothing()
    End Sub

    Public Sub Configuring_a_function_to_return_a_value()
        Dim factory = A.Fake(Of IWidgetFactory)()

        A.CallTo(Function() factory.Create()).Returns(A.Fake(Of IWidget)())
    End Sub
End Class
