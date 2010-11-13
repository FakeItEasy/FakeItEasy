Imports FakeItEasy


Public Class RaisingEvents
    Public Sub Raising_an_event()
        Dim widget = A.Fake(Of IWidget)()

        ' Raise with the shorter syntax, works only for EventHandler(Of T).
        AddHandler widget.WidgetBroke, Raise.With(New WidgetEventArgs("foo")).Go

        ' Raise with the C# syntax
        AddHandler widget.WidgetBroke, AddressOf Raise.With(New WidgetEventArgs("foo")).Now
    End Sub
End Class