namespace FakeItEasy.Tests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class TypeExtensionsTests
    {
        private interface IFoo
        {
        }

        private interface IBar
        {
        }

        [Fact]
        public void CanBeInstantiatedAs_should_return_false_when_type_is_not_assignable_from_targetType()
        {
            typeof(Bar).CanBeInstantiatedAs(typeof(IFoo)).Should().BeFalse();
        }

        [Fact]
        public void CanBeInstantiatedAs_should_return_false_when_type_is_an_interface()
        {
            typeof(IBar).CanBeInstantiatedAs(typeof(IBar)).Should().BeFalse();
        }

        [Fact]
        public void CanBeInstantiatedAs_should_return_false_when_type_is_abstract()
        {
            typeof(AbstractBar).CanBeInstantiatedAs(typeof(IBar)).Should().BeFalse();
        }

        [Fact]
        public void CanBeInstantiatedAs_should_return_false_when_type_is_an_open_generic()
        {
            typeof(GenericBar<>).CanBeInstantiatedAs(typeof(AbstractBar)).Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(Bar), typeof(IBar))]
        [InlineData(typeof(Bar), typeof(AbstractBar))]
        [InlineData(typeof(Bar), typeof(Bar))]
        [InlineData(typeof(GenericBar<IFoo>), typeof(IBar))]
        [InlineData(typeof(GenericBar<IFoo>), typeof(AbstractBar))]
        [InlineData(typeof(GenericBar<IFoo>), typeof(GenericBar<IFoo>))]
        public void CanBeInstantiatedAs_should_return_true_when_type_can_be_instantiated_as_targetType(
            Type type,
            Type targetType)
        {
            type.CanBeInstantiatedAs(targetType).Should().BeTrue();
        }

        private abstract class AbstractBar : IBar
        {
        }

        private class Bar : AbstractBar
        {
        }

        private class GenericBar<T> : AbstractBar
        {
        }
    }
}
