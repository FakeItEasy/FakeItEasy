namespace FakeItEasy.Tests
{
    using System;
    using System.Linq;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class ReturnsNextFromSequenceExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Should_call_returns_with_factory_that_returns_next_from_sequence_for_each_call()
        {
            // Arrange
            var sequence = new[] { 1, 2, 3 };
            var config = A.Fake<IReturnValueConfiguration<int>>();
            var call = A.Fake<IFakeObjectCall>();

            // Act
            config.ReturnsNextFromSequence(sequence);

            // Assert
            Func<Func<IFakeObjectCall, int>> factoryValidator = () => A<Func<IFakeObjectCall, int>>.That.Matches(
                x =>
                {
                    var producedSequence = new[] { x.Invoke(call), x.Invoke(call), x.Invoke(call) };
                    return producedSequence.SequenceEqual(sequence);
                },
                "Predicate");

            A.CallTo(() => config.ReturnsLazily(factoryValidator.Invoke())).MustHaveHappened();
        }

        [Test]
        public void Should_set_repeat_to_the_number_of_values_in_sequence()
        {
            // Arrange
            var config = A.Fake<IReturnValueConfiguration<int>>();
            var returnedConfig = A.Fake<IAfterCallSpecifiedWithOutAndRefParametersConfiguration>();

            A.CallTo(() => config.ReturnsLazily(A<Func<IFakeObjectCall, int>>._)).Returns(returnedConfig);

            // Act
            config.ReturnsNextFromSequence(1, 2, 3);

            // Assert
            A.CallTo(() => returnedConfig.NumberOfTimes(3)).MustHaveHappened();
        }
    }
}
