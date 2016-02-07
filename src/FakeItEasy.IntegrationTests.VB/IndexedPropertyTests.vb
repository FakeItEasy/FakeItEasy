Imports FluentAssertions
Imports NUnit.Framework

Public Interface IHaveAnInterestingProperty

    Property IndexedProperty(ByVal index As Integer) As Object

End Interface

<TestFixture()> _
Public Class IndexedPropertyTests

    <Test()> _
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

    <Test()> _
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

End Class
