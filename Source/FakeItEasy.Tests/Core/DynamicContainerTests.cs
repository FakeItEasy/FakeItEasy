namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicContainerTests
    {
        private List<IDummyFactory> availableDummyFactories;
        private List<IFakeOptionsBuilder> availableOptionsBuilders;

        private IDisposable scope;

        [SetUp]
        public void Setup()
        {
            this.scope = Fake.CreateScope(new NullFakeObjectContainer());

            this.availableOptionsBuilders = new List<IFakeOptionsBuilder>();
            this.availableDummyFactories = new List<IDummyFactory>();
        }

        [TearDown]
        public void Teardown()
        {
            this.scope.Dispose();
        }

        [Test]
        public void TryCreateFakeObject_should_create_fake_for_type_that_has_factory()
        {
            this.availableDummyFactories.Add(new DummyFactoryForTypeWithFactory());

            var container = this.CreateContainer();

            object fake;

            container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out fake).Should().BeTrue();
            fake.Should().BeOfType<TypeWithDummyFactory>();
        }

        [Test]
        public void TryCreateFakeObject_should_return_false_when_no_factory_exists()
        {
            var container = this.CreateContainer();

            object fake;

            container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out fake).Should().BeFalse();
        }

        [Test]
        public void ConfigureFake_should_apply_configuration_from_registered_options_builders()
        {
            this.availableOptionsBuilders.Add(new OptionsBuilderForTypeWithDummyFactory());

            var container = this.CreateContainer();

            var fakeOptions = A.Fake<IFakeOptions>();
            var fakeTypeWithDummyFactory = A.Fake<TypeWithDummyFactory>();

            A.CallTo(() => fakeOptions.ConfigureFake(A<Action<object>>._))
                .Invokes((Action<object> action) => action(fakeTypeWithDummyFactory));

            container.BuildOptions(typeof(TypeWithDummyFactory), fakeOptions);

            Assert.That(fakeTypeWithDummyFactory.WasConfigured, Is.True, "configuration was not applied");
        }

        [Test]
        public void ConfigureFake_should_do_nothing_when_fake_type_has_no_options_builder_specified()
        {
            this.CreateContainer();
            A.Fake<TypeWithDummyFactory>();
        }

        [Test]
        public void Should_not_fail_when_more_than_one_factory_exists_for_a_given_type()
        {
            // Arrange
            this.availableDummyFactories.Add(new DummyFactoryForTypeWithFactory());
            this.availableDummyFactories.Add(new DuplicateDummyFactoryForTypeWithFactory());

            var container = this.CreateContainer();

            // Act
            object fake;
            var result = container.TryCreateDummyObject(typeof(TypeWithDummyFactory), out fake);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void Should_not_fail_when_more_than_one_options_builder_exists_for_a_given_type()
        {
            // Arrange
            this.availableOptionsBuilders.Add(new OptionsBuilderForTypeWithDummyFactory());
            this.availableOptionsBuilders.Add(new DuplicateOptionsBuilderForTypeWithDummyFactory());

            // Act

            // Assert
            Assert.DoesNotThrow(() =>
                this.CreateContainer());
        }

        private DynamicContainer CreateContainer()
        {
            return new DynamicContainer(this.availableDummyFactories, this.availableOptionsBuilders);
        }

        public class OptionsBuilderForTypeWithDummyFactory : IFakeOptionsBuilder
        {
            public Priority Priority
            {
                get { return Priority.Default; }
            }

            public bool CanBuildOptionsForFakeOfType(Type type)
            {
                return type == typeof(TypeWithDummyFactory);
            }

            public void BuildOptions(Type typeOfFake, IFakeOptions options)
            {
                if (options == null)
                {
                    return;
                }

                options.ConfigureFake(fake => A.CallTo(() => ((TypeWithDummyFactory)fake).WasConfigured).Returns(true));
            }
        }

        public class DuplicateOptionsBuilderForTypeWithDummyFactory : IFakeOptionsBuilder
        {
            public Priority Priority
            {
                get { return Priority.Default; }
            }

            public bool CanBuildOptionsForFakeOfType(Type type)
            {
                return type == typeof(TypeWithDummyFactory);
            }

            public void BuildOptions(Type typeOfFake, IFakeOptions options)
            {
                if (options == null)
                {
                    return;
                }

                options.ConfigureFake(fake => A.CallTo(() => ((TypeWithDummyFactory)fake).WasConfigured).Returns(true));
            }
        }

        public class DummyFactoryForTypeWithFactory : DummyFactory<TypeWithDummyFactory>
        {
            protected override TypeWithDummyFactory Create()
            {
                return new TypeWithDummyFactory();
            }
        }

        public class DuplicateDummyFactoryForTypeWithFactory : DummyFactory<TypeWithDummyFactory>
        {
            protected override TypeWithDummyFactory Create()
            {
                return new TypeWithDummyFactory();
            }
        }

        public class TypeWithDummyFactory
        {
            public virtual bool WasConfigured
            {
                get { return false; }
            }
        }
    }
}
