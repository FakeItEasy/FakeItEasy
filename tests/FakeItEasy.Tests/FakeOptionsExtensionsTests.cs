namespace FakeItEasy.Tests
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class FakeOptionsExtensionsTests
    {
        [Fact]
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

        [Fact]
        public void Strict_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () => A.Dummy<IFakeOptions<IFoo>>().Strict();
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void CallsBaseMethods_should_configure_fake_to_call_concrete_base_method()
        {
            // Arrange
            var fake = A.Fake<ConcreteClass>(x => x.CallsBaseMethods());

            // Act
            var result = fake.Method();

            // Assert
            result.Should().Be(17);
        }

        [Fact]
        public void CallsBaseMethods_should_not_configure_fake_to_call_abstract_base_method()
        {
            // Arrange
            var fake = A.Fake<IFoo>(x => x.CallsBaseMethods());

            // Act
            var result = fake.Baz();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
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

        [Fact]
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
