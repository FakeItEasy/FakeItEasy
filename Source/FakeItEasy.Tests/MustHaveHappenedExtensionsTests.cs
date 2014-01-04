namespace FakeItEasy.Tests
{
    using System.Linq;
    using FakeItEasy.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class MustHaveHappenedExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Should_call_configuration_with_repeat_once()
        {
            // Arrange
            var configuration = A.Fake<IAssertConfiguration>();

            // Act
            configuration.MustHaveHappened();

            // Assert
            A.CallTo(() => configuration.MustHaveHappened(A<Repeated>.That.Matches(x => x.Matches(1))))
                .MustHaveHappened(Repeated.Exactly.Once); // avoid .MustHaveHappened(), since we're testing it
        }

        [Test]
        public void Should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IAssertConfiguration>().MustHaveHappened());
        }
    }
}