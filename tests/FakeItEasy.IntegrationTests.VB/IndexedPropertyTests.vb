Imports FluentAssertions
Imports Xunit

Public Interface IHaveAnInterestingProperty

    Property IndexedProperty(ByVal index As Integer) As Object

End Interface

Public Class IndexedPropertyTests

    <Fact> _
    Public Sub Set_followed_by_get_should_return_same_object()
        'Arrange
        Dim target = A.Fake(Of IHaveAnInterestingProperty)()
        Dim initialValue = New Object()

        'Act
        target.IndexedProperty(3) = initialValue

        Dim fetchedValue = target.IndexedProperty(3)

        'Assert
        AssertionExtensions.Should(ReferenceEquals(fetchedValue, initialValue)).BeTrue()
    End Sub

    <Fact> _
    Public Sub Set_followed_by_get_with_different_index_should_return_a_different_object()
        'Arrange
        Dim target = A.Fake(Of IHaveAnInterestingProperty)()
        Dim initialValue = New Object()

        'Act
        target.IndexedProperty(3) = initialValue

        Dim fetchedValue = target.IndexedProperty(4)

        'Assert
        AssertionExtensions.Should(ReferenceEquals(fetchedValue, initialValue)).BeFalse()
    End Sub

    <Fact> _
    Public Sub Set_configured_with_any_value_followed_by_get_with_different_index_should_not_trigger_configured_action()
        'Arrange
        Dim wasInvoked = false
        Dim target = A.Fake(Of IHaveAnInterestingProperty)()
        A.CallToSet(Function() target.IndexedProperty(3)).Invokes(Sub() wasInvoked = true )

        'Act
        target.IndexedProperty(4) = New Object()

        'Assert
        wasInvoked.Should().BeFalse
    End Sub

    <Fact> _
    Public Sub Set_configured_with_any_value_followed_by_get_with_same_index_should_trigger_configured_action()
        'Arrange
        Dim wasInvoked = false
        Dim target = A.Fake(Of IHaveAnInterestingProperty)()
        A.CallToSet(Function() target.IndexedProperty(3)).Invokes(Sub() wasInvoked = true )

        'Act
        target.IndexedProperty(3) = New Object()

        'Assert
        wasInvoked.Should().BeTrue
    End Sub

End Class
