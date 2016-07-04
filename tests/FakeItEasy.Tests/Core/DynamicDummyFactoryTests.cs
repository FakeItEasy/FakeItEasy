namespace FakeItEasy.Tests.Core
{
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class DynamicDummyFactoryTests
    {
        private readonly List<IDummyFactory> availableDummyFactories;

        public DynamicDummyFactoryTests()
        {
            this.availableDummyFactories = new List<IDummyFactory>();
        }

        [Fact]
        public void TryCreateDummyObject_should_create_dummy_for_type_that_has_factory()
        {
            this.availableDummyFactories.Add(new DummyFactoryForTypeWithFactory());

            var container = this.CreateFactory();

            object dummy;

            container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out dummy).Should().BeTrue();
            dummy.Should().BeOfType<TypeWithDummyFactory>();
        }

        [Fact]
        public void TryCreateDummyObject_should_return_false_when_no_factory_exists()
        {
            var container = this.CreateFactory();

            object dummy;

            container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out dummy).Should().BeFalse();
        }

        [Fact]
        public void TryCreateDummyObject_should_not_fail_when_more_than_one_factory_exists_for_a_given_type()
        {
            // Arrange
            this.availableDummyFactories.Add(new DummyFactoryForTypeWithFactory());
            this.availableDummyFactories.Add(new DuplicateDummyFactoryForTypeWithFactory());

            var container = this.CreateFactory();

            // Act
            object dummy;
            var result = container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out dummy);

            // Assert
            result.Should().BeTrue();
        }

        private DynamicDummyFactory CreateFactory()
        {
            return new DynamicDummyFactory(this.availableDummyFactories);
        }

        private class DummyFactoryForTypeWithFactory : DummyFactory<TypeWithDummyFactory>
        {
            protected override TypeWithDummyFactory Create()
            {
                return new TypeWithDummyFactory();
            }
        }

        private class DuplicateDummyFactoryForTypeWithFactory : DummyFactory<TypeWithDummyFactory>
        {
            protected override TypeWithDummyFactory Create()
            {
                return new TypeWithDummyFactory();
            }
        }

        private class TypeWithDummyFactory
        {
            public virtual bool WasConfigured => false;
        }
    }
}
