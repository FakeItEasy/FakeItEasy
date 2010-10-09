namespace FakeItEasy.Tests
{
    using System.Collections.Generic;
    using FakeItEasy.Expressions.ArgumentConstraints;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentConstraintExtensionsTests
    {
        [Test]
        public void Contains_should_return_collection_contains_constraint()
        {
            // Arrange
            var constraint = A<List<string>>.That.Contains("Foo");

            // Act

            // Assert
            Assert.That(constraint, Is.InstanceOf<EnumerableContainsConstraint<List<string>>>());
        }
    }
}
