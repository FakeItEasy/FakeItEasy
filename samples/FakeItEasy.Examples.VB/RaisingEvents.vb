Imports FakeItEasy.Examples.ExampleObjects

Public Class RaisingEvents
    Public Sub Raising_an_event()
        Dim widget = A.Fake(Of IWidget)()

        ' Raise EventHandler(Of T)
        AddHandler widget.WidgetBroke, Raise.With(New WidgetEventArgs("foo"))

        ' Raise EventHandler
        AddHandler widget.WidgetRunning, Raise.With(New EventArgs())
    End Sub
End Class