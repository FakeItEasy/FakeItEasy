namespace FakeItEasy.Tests;

using FakeItEasy.Tests.TestHelpers.FSharp;
using FluentAssertions;
using Xunit;

public class MethodInfoExtensionsTests
{
    [Fact]
    public void GetDescription_AnonymousParameters_OmitsParameterNames()
    {
        // F# (at least) allows users to declare methods with anonymous parameters, which requires
        // special handling to render in a pleasing way.
        // Attempts to trigger the method-rendering from specs have failed so far, as it requires
        // generating a method with anonymous types that can't be intercepted, on a type that can
        // be. But just in case we're not clever enough to find a way, and some client does, check
        // the rendering with a unit test.

        // Arrange
        var expected = "FakeItEasy.Tests.TestHelpers.FSharp.IHaveAMethodWithAnAnonymousParameter.Save(System.Int32)";

        // Act
        var actual = typeof(IHaveAMethodWithAnAnonymousParameter).GetMethod("Save")!.GetDescription();

        // Assert
        actual.Should().Be(expected);
    }
}
