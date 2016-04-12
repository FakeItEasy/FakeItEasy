namespace FakeItEasy.Tests
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class FakeOptionsExtensionsTests
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
            var options = A.Fake<IFakeOptions<IFoo>>();
            A.CallTo(() => options.ConfigureFake(A<Action<IFoo>>._)).Returns(options);

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
            Expression<Action> call = () => A.Dummy<IFakeOptions<IFoo>>().Strict();
            call.Should().BeNullGuarded();
        }

        [Test]
        public void CallsBaseMethods_should_configure_fake_to_call_concrete_base_method()
        {
            // Arrange
            var fake = A.Fake<ConcreteClass>(x => x.CallsBaseMethods());

            // Act
            var result = fake.Method();

            // Assert
            result.Should().Be(17);
        }

        [Test]
        public void CallsBaseMethods_should_not_configure_fake_to_call_abstract_base_method()
        {
            // Arrange
            var fake = A.Fake<IFoo>(x => x.CallsBaseMethods());

            // Act
            var result = fake.Baz();

            // Assert
            result.Should().Be(0);
        }

        [Test]
        public void CallsBaseMethods_should_return_configuration_object()
        {
            // Arrange
            var options = A.Fake<IFakeOptions<IFoo>>();
            A.CallTo(() => options.ConfigureFake(A<Action<IFoo>>._)).Returns(options);

            // Act
            var result = options.CallsBaseMethods();

            // Assert
            result.Should().BeSameAs(options);
        }

        [Test]
        public void CallsBaseMethods_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Dummy<IFakeOptions<IFoo>>().CallsBaseMethods();
            call.Should().BeNullGuarded();
        }

        public class ConcreteClass
        {
            public virtual int Method()
            {
                return 17;
            }
        }
    }
}
