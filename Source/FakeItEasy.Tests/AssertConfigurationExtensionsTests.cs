namespace FakeItEasy.Tests
{
    using System.Linq;
    using FakeItEasy.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class MustHaveHappenedExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void MustHaveHappened_should_call_configuration_with_repeat_once()
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
        public void MustHaveHappened_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IAssertConfiguration>().MustHaveHappened());
        }

        [TestCase(0, Result = true)]
        [TestCase(1, Result = false)]
        [TestCase(3, Result = false)]
        public bool MustNotHaveHappened_should_call_configuration_with_repeat_that_validates_correctly(int repeat)
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
        public void MustNotHaveHappened_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IAssertConfiguration>().MustNotHaveHappened());
        }
    }
}