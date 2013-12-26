namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class ReturnsExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Should_return_configuration_returned_from_passed_in_configuration()
        {
            // Arrange
            var expectedConfig = A.Fake<IAfterCallSpecifiedWithOutAndRefParametersConfiguration>();
            var config = A.Fake<IReturnValueConfiguration<int>>();
            A.CallTo(() => config.ReturnsLazily(A<Func<IFakeObjectCall, int>>.That.Matches(x => x.Invoke(null) == 10))).Returns(expectedConfig);

            // Act
            var returned = config.Returns(10);

            // Assert
            returned.Should().BeSameAs(expectedConfig);
        }

        [Test]
        public void Should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Fake<IReturnValueConfiguration<string>>().Returns(null));
        }
    }
}
