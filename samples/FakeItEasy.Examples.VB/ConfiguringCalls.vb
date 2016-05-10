Imports FakeItEasy.Examples.ExampleObjects

Public Class ConfiguringCalls
    Public Sub Configuring_a_sub_using_lambda()
        Dim widget = A.Fake(Of IWidget)()

        A.CallTo(Sub() widget.Repair()).DoesNothing()
    End Sub

    Public Sub Configuring_a_function_to_return_a_value()
        Dim factory = A.Fake(Of IWidgetFactory)()

        A.CallTo(Function() factory.Create()).Returns(A.Fake(Of IWidget)())
    End Sub

    Public Sub Configuring_a_property_setter_and_specifying_a_matching_value()
        Dim widget = A.Fake(Of IWidget)()

        A.CallToSet(Function() widget.Name).To("cog").DoesNothing()
    End Sub
End Class
