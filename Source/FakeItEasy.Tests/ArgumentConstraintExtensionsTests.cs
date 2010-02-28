namespace FakeItEasy.Tests
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using NUnit.Framework.Constraints;
    using FakeItEasy.Expressions.ArgumentConstraints;

    [TestFixture]
    public class ArgumentConstraintExtensionsTests
    {
        [Test]
        public void Contains_should_return_collection_contains_constraint()
        {
            // Arrange
            var constraint = A<List<string>>.That.Contains(10);

            // Act

            // Assert
            Assert.That(constraint, Is.InstanceOf<EnumerableContainsConstraint<List<string>>>());
        }
    }
}
