namespace FakeItEasy.Tests.Expressions
{
    using FakeItEasy.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentConstraintExtensionsTests
    {
        [Test]
        public void Or_should_pass_root_scope_to_delegate()
        {
            // Arrange
            ArgumentConstraintScope<string> scopePassedToDelegate = null;

            // Act
            A<string>.That.IsNull().Or(x =>
            {
                scopePassedToDelegate = x;
                return scopePassedToDelegate.Contains("foo");
            });

            // Assert
            Assert.That(scopePassedToDelegate, Is.InstanceOf<RootValidations<string>>());
        }

        [Test]
        public void Or_should_return_or_validator()
        {
            // Arrange
            
            // Act
            var returned = A<string>.That.IsNull().Or(x => x.Contains("foo"));
            
            // Assert
            Assert.That(returned, Is.Not.Null);
        }

        [Test]
        public void Or_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A<string>.That.StartsWith("foo").Or(x => x.Contains("bar")));
        }
    }
}
