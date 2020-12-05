Imports System.Diagnostics.CodeAnalysis
Imports FluentAssertions
Imports Xunit

Public Interface IHaveEvents

    Event NonGenericEventHander As EventHandler
    Event GenericEventHander As EventHandler(Of MyEventArgs)

    Event TokenEvent(ByVal eventValue As Token)

End Interface

Public Class MyEventArgs
    Inherits EventArgs
End Class

Public Class Token
End Class

Public Class RaisingEventsTests

    Dim capturedSender As Object
    Dim capturedEventArgs As EventArgs
    Dim capturedToken As Token

    Private Sub HandlesNonGenericEventHandler(sender As Object, eventArgs As EventArgs)
        capturedSender = sender
        capturedEventArgs = eventArgs
    End Sub

    Private Sub HandlesGenericEventHandler(sender As Object, eventArgs As MyEventArgs)
        capturedSender = sender
        capturedEventArgs = eventArgs
    End Sub

    Private Sub HandlesTokenEvent(token As Token)
        capturedToken = token
    End Sub

    Public Sub New()
        capturedSender = Nothing
        capturedEventArgs = Nothing
    End Sub

    <Fact>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandler_sends_good_sender()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim aSender = New Object

        AddHandler target.NonGenericEventHander, AddressOf HandlesNonGenericEventHandler

        ' Act
        AddHandler target.NonGenericEventHander, Raise.With(aSender, New EventArgs())

        ' Assert
        CapturedSenderShouldBeSameAs(aSender)
    End Sub

    <Fact>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandler_sends_good_arguments()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim eventArgs = New EventArgs()
        Dim aSender = New Object

        AddHandler target.NonGenericEventHander, AddressOf HandlesNonGenericEventHandler

        ' Act
        AddHandler target.NonGenericEventHander, Raise.With(aSender, eventArgs)

        ' Assert
        capturedEventArgs.Should().BeSameAs(eventArgs)
    End Sub

    <Fact>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandlerOfT_sends_good_sender()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim aSender = New Object

        AddHandler target.GenericEventHander, AddressOf HandlesGenericEventHandler

        ' Act
        AddHandler target.GenericEventHander, Raise.With(aSender, New MyEventArgs())

        ' Assert
        CapturedSenderShouldBeSameAs(aSender)
    End Sub

    <Fact>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandlerOfT_sends_good_arguments()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim eventArgs = New MyEventArgs()
        Dim aSender = New Object

        AddHandler target.GenericEventHander, AddressOf HandlesGenericEventHandler

        ' Act
        AddHandler target.GenericEventHander, Raise.With(aSender, eventArgs)

        ' Assert
        capturedEventArgs.Should().BeSameAs(eventArgs)
    End Sub

    <Fact>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
        Justification:="Required for testing.")>
    Public Sub RaiseFreeform_TokenEvent_sends_token()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim token As Token = New Token

        AddHandler target.TokenEvent, AddressOf HandlesTokenEvent

        ' Act
        AddHandler target.TokenEvent, Raise.FreeForm(Of IHaveEvents.TokenEventEventHandler).With(token)

        ' Assert
        capturedToken.Should().BeSameAs(token)
    End Sub

    Private Sub CapturedSenderShouldBeSameAs(aSender As Object)
        ' We cannot use fluent assertions on Object because VB.NET doesn't support extension methods on Object;
        ' this is related to the fact that there is no dynamic in VB.NET: anything typed as Object is late bound.
        ' More details at https://blogs.msdn.microsoft.com/vbteam/2007/01/24/extension-methods-and-late-binding-extension-methods-part-4/
        ReferenceEquals(capturedSender, aSender).Should().BeTrue()
    End Sub

End Class
