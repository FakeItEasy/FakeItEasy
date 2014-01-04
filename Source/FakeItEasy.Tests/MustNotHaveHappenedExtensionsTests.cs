namespace FakeItEasy.Tests
{
    using System.Linq;
    using FakeItEasy.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class MustNotHaveHappenedExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [TestCase(0, Result = true)]
        [TestCase(1, Result = false)]
        [TestCase(3, Result = false)]
        public bool Should_call_configuration_with_repeat_that_validates_correctly(int repeat)
        {
            // Arrange
            var configuration = A.Fake<IAssertConfiguration>();

            // Act
            configuration.MustNotHaveHappened();

            // Assert
            var specifiedRepeat = Fake.GetCalls(configuration).Single().Arguments.Get<Repeated>(0);
            return specifiedRepeat.Matches(repeat);
        }

        [Test]
        public void Should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IAssertConfiguration>().MustNotHaveHappened());
        }
    }
}