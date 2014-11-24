Imports System.Diagnostics.CodeAnalysis
Imports FluentAssertions
Imports NUnit.Framework

<Assembly: SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances",
    Scope:="type", Target:="FakeItEasy.IntegrationTests.VB.IHaveEvents+DerivedEventHanderEventHandler",
    Justification:="Required to test nonstandard events.")> 

Public Interface IHaveEvents

    Event NonGenericEventHander As EventHandler
    Event GenericEventHander As EventHandler(Of MyEventArgs)

    <SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
        Justification:="Required to test nonstandard events.")>
    Event DerivedEventHander(ByVal sender As Object, args As MyEventArgs)

    <SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
        Justification:="Required to test nonstandard events.")>
    Event ObjectEvent(ByVal eventValue As Object)

End Interface

Public Class MyEventArgs
    Inherits EventArgs
End Class

<TestFixture()>
Public Class RaisingEventsTests

    Dim capturedSender As Object
    Dim capturedEventArgs As EventArgs
    Dim capturedObject As Object

    Private Sub HandlesNonGenericEventHandler(sender As Object, eventArgs As EventArgs)
        capturedSender = sender
        capturedEventArgs = eventArgs
    End Sub

    Private Sub HandlesGenericEventHandler(sender As Object, eventArgs As MyEventArgs)
        capturedSender = sender
        capturedEventArgs = eventArgs
    End Sub

    Private Sub HandlesDerivedEventHandler(sender As Object, eventArgs As MyEventArgs)
        capturedSender = sender
        capturedEventArgs = eventArgs
    End Sub

    Private Sub HandlesObjectEvent(objectOfInterest As Object)
        capturedObject = objectOfInterest
    End Sub

    <SetUp()>
    Public Sub Setup()
        capturedSender = Nothing
        capturedEventArgs = Nothing
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
    Public Sub Raise_DerivedEventHandler_sends_good_sender()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim aSender = New Object

        AddHandler target.DerivedEventHander, AddressOf HandlesDerivedEventHandler

        ' Act
        AddHandler target.DerivedEventHander, Raise.With(Of IHaveEvents.DerivedEventHanderEventHandler)(aSender, New MyEventArgs())

        ' Assert
        ReferenceEquals(capturedSender, aSender).Should().BeTrue()
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification:="Required for testing.")>
    Public Sub Raise_DerivedEventHandler_sends_good_arguments()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim eventArgs = New MyEventArgs()
        Dim aSender = New Object

        AddHandler target.DerivedEventHander, AddressOf HandlesDerivedEventHandler

        ' Act
        AddHandler target.DerivedEventHander, Raise.With(Of IHaveEvents.DerivedEventHanderEventHandler)(aSender, eventArgs)

        ' Assert
        ReferenceEquals(capturedEventArgs, eventArgs).Should().BeTrue()
    End Sub

    <Test()>
    <SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
        Justification:="Required for testing.")>
    Public Sub Raise_ObjectEvent_sends_object()
        'Arrange
        Dim target = A.Fake(Of IHaveEvents)()
        Dim anObject As Object = New Object()

        AddHandler target.ObjectEvent, AddressOf HandlesObjectEvent

        ' Act
        AddHandler target.ObjectEvent, Raise.With(Of IHaveEvents.ObjectEventEventHandler)(anObject)

        ' Assert
        ReferenceEquals(capturedObject, anObject).Should().BeTrue()
    End Sub
End Class
