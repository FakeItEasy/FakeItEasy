namespace FakeItEasy.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class StrictExtensionsTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void Strict_should_configure_fake_to_throw_expectation_exception()
        {
            // Arrange
            var foo = A.Fake<IFoo>(x => x.Strict());

            // Act
            var exception = Record.Exception(() => foo.Bar());

            // Assert
            exception.Should()
                .BeAnExceptionOfType<ExpectationException>()
                .WithMessage("Call to non configured method \"Bar\" of strict fake.");
        }

        [Test]
        public void Strict_should_return_configuration_object()
        {
            // Arrange
            var options = A.Fake<IFakeOptionsBuilder<IFoo>>();
            A.CallTo(() => options.OnFakeCreated(A<Action<IFoo>>._)).Returns(options);

            // Act
            var result = options.Strict();

            // Assert
            result.Should().BeSameAs(options);
        }

        [Test]
        public void Strict_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                A.Dummy<IFakeOptionsBuilder<IFoo>>().Strict());
        }
    }
}
