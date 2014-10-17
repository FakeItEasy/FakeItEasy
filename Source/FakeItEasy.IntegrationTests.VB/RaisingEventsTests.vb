Imports System.Diagnostics.CodeAnalysis
Imports FluentAssertions
Imports NUnit.Framework

Public Interface IHaveEvents

    Event NonGenericEventHander As EventHandler
    Event GenericEventHander As EventHandler(Of MyEventArgs)

End Interface

Public Class MyEventArgs
    Inherits EventArgs
End Class

<TestFixture()>
Public Class RaisingEventsTests

    Dim capturedSender As Object
    Dim capturedEventArgs As EventArgs

    Private Sub HandlesNonGenericEventHandler(sender As Object, eventArgs As EventArgs)
        capturedSender = sender
        capturedEventArgs = eventArgs
    End Sub

    Private Sub HandlesGenericEventHandler(sender As Object, eventArgs As MyEventArgs)
        capturedSender = sender
        capturedEventArgs = eventArgs
    End Sub

    <SetUp()>
    Public Sub Setup()
        capturedSender = Nothing
        capturedEventArgs = Nothing
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandler_with_now_sends_good_sender()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim aSender = New Object

        AddHandler target.NonGenericEventHander, AddressOf HandlesNonGenericEventHandler

        ' Act
        AddHandler target.NonGenericEventHander, AddressOf Raise.With(aSender, New EventArgs()).Now

        ' Assert
        ReferenceEquals(capturedSender, aSender).Should().BeTrue()
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandler_with_now_sends_good_arguments()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim eventArgs = New EventArgs()
        Dim aSender = New Object

        AddHandler target.NonGenericEventHander, AddressOf HandlesNonGenericEventHandler

        ' Act
        AddHandler target.NonGenericEventHander, AddressOf Raise.With(aSender, eventArgs).Now

        ' Assert
        ReferenceEquals(capturedEventArgs, eventArgs).Should().BeTrue()
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandler_sends_good_sender()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim aSender = New Object

        AddHandler target.NonGenericEventHander, AddressOf HandlesNonGenericEventHandler

        ' Act
        AddHandler target.NonGenericEventHander, Raise.With(aSender, New EventArgs())

        ' Assert
        ReferenceEquals(capturedSender, aSender).Should().BeTrue()
    End Sub

    <Test()>
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
        ReferenceEquals(capturedEventArgs, eventArgs).Should().BeTrue()
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandlerOfT_with_now_sends_good_sender()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim aSender = New Object

        AddHandler target.GenericEventHander, AddressOf HandlesGenericEventHandler

        ' Act
        AddHandler target.GenericEventHander, AddressOf Raise.With(aSender, New MyEventArgs()).Now

        ' Assert
        ReferenceEquals(capturedSender, aSender).Should().BeTrue()
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandlerOfT_with_now_sends_good_arguments()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim eventArgs = New MyEventArgs()
        Dim aSender = New Object

        AddHandler target.GenericEventHander, AddressOf HandlesGenericEventHandler

        ' Act
        AddHandler target.GenericEventHander, AddressOf Raise.With(aSender, eventArgs).Now

        ' Assert
        ReferenceEquals(capturedEventArgs, eventArgs).Should().BeTrue()
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandlerOfT_sends_good_sender()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim aSender = New Object

        AddHandler target.GenericEventHander, AddressOf HandlesGenericEventHandler

        ' Act
        AddHandler target.GenericEventHander, Raise.With(aSender, New MyEventArgs())

        ' Assert
        ReferenceEquals(capturedSender, aSender).Should().BeTrue()
    End Sub

    <Test()>
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
        ReferenceEquals(capturedEventArgs, eventArgs).Should().BeTrue()
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandlerOfT_with_go_sends_good_sender()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim aSender = New Object

        AddHandler target.GenericEventHander, AddressOf HandlesGenericEventHandler

        ' Act
        AddHandler target.GenericEventHander, Raise.With(aSender, New MyEventArgs()).Go

        ' Assert
        ReferenceEquals(capturedSender, aSender).Should().BeTrue()
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_EventHandlerOfT_with_go_sends_good_arguments()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim eventArgs = New MyEventArgs()
        Dim aSender = New Object

        AddHandler target.GenericEventHander, AddressOf HandlesGenericEventHandler

        ' Act
        AddHandler target.GenericEventHander, Raise.With(aSender, eventArgs).Go

        ' Assert
        ReferenceEquals(capturedEventArgs, eventArgs).Should().BeTrue()
    End Sub

End Class
