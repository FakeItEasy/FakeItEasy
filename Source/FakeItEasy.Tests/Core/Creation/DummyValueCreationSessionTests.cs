namespace FakeItEasy.Tests.Core.Creation
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class DummyValueCreationSessionTests
    {
        private IFakeObjectContainer container;
        private IFakeObjectCreator fakeObjectCreator;
        private DummyValueCreationSession session;

        [SetUp]
        public void SetUp()
        {
            this.container = A.Fake<IFakeObjectContainer>();
            this.fakeObjectCreator = A.Fake<IFakeObjectCreator>();

            this.session = new DummyValueCreationSession(this.container, this.fakeObjectCreator);
        }

        [Test]
        public void Should_return_dummy_from_container_when_available()
        {
            // Arrange
            this.StubContainerWithValue<string>("dummy value");

            // Act
            object dummy;
            var result = this.session.TryResolveDummyValue(typeof(string), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.EqualTo("dummy value"));
        }

        [Test]
        public void Should_return_false_when_type_can_not_be_created()
        {
            // Arrange

            // Act
            object dummy = null;
            var result = this.session.TryResolveDummyValue(typeof(TypeThatCanNotBeInstantiated), out dummy);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(dummy, Is.Null);
        }

        [Test]
        public void Should_return_fake_when_it_can_be_created()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            this.StubFakeObjectCreatorWithValue<IFoo>(fake);

            // Act
            object dummy;
            var result = this.session.TryResolveDummyValue(typeof(IFoo), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.SameAs(fake));
        }

        [Test]
        public void Should_return_default_value_when_type_is_value_type()
        {
            // Arrange

            // Act
            object dummy;
            var result = this.session.TryResolveDummyValue(typeof(int), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.EqualTo(0));
        }

        [Test]
        public void Should_be_able_to_create_class_with_default_constructor()
        {
            // Arrange

            // Act
            object dummy;
            var result = this.session.TryResolveDummyValue(typeof(ClassWithDefaultConstructor), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.InstanceOf<ClassWithDefaultConstructor>());
        }

        [Test]
        public void Should_be_able_to_create_class_with_resolvable_constructor_arguments()
        {
            // Arrange
            this.StubContainerWithValue<string>("dummy string");
            this.StubFakeObjectCreatorWithValue(A.Fake<IFoo>());

            // Act
            object dummy;
            var result = this.session.TryResolveDummyValue(typeof(TypeWithResolvableConstructorArguments<string, IFoo>), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.InstanceOf<TypeWithResolvableConstructorArguments<string, IFoo>>());
        }

        [Test]
        public void Should_not_be_able_to_create_class_with_circular_dependencies()
        {
            // Arrange

            // Act
            object dummy;
            var result = this.session.TryResolveDummyValue(typeof(TypeWithCircularDependency), out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_be_able_to_resolve_same_type_twice_when_successful()
        {
            // Arrange
            this.StubContainerWithValue<string>("dummy value");

            object dummy;
            this.session.TryResolveDummyValue(typeof(string), out dummy);
            dummy = null;

            // Act
            var result = this.session.TryResolveDummyValue(typeof(string), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.EqualTo("dummy value"));
        }

        [Test]
        public void Should_return_false_when_default_constructor_throws()
        {
            // Arrange

            // Act
            object dummy;
            var result = this.session.TryResolveDummyValue(typeof(TypeWithDefaultConstructorThatThrows), out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_favour_the_widest_constructor_when_activating()
        {
            // Arrange
            this.StubContainerWithValue<string>("dummy value");

            // Act
            object dummy;
            this.session.TryResolveDummyValue(typeof(TypeWithMultipleConstructorsOfDifferentWidth), out dummy);
            var typedDummy = (TypeWithMultipleConstructorsOfDifferentWidth)dummy;

            // Assert
            Assert.That(typedDummy.WidestConstructorWasCalled, Is.True);
        }

        [TestCase(typeof(void))]
        [TestCase(typeof(Func<int>))]
        [TestCase(typeof(Action))]
        public void Should_return_false_for_restricted_types(Type restrictedType)
        {
            // Arrange

            // Act
            object dummy;
            var result = this.session.TryResolveDummyValue(restrictedType, out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        private void StubFakeObjectCreatorWithValue<T>(T value)
        {
            object output;
            A.CallTo(() => this.fakeObjectCreator.TryCreateFakeObject(typeof(T), this.session, out output))
                .Returns(true)
                .AssignsOutAndRefParameters(value);
        }

        private void StubContainerWithValue<T>(T value)
        {
            object output;
            A.CallTo(() => this.container.TryCreateDummyObject(typeof(T), out output))
                .Returns(true)
                .AssignsOutAndRefParameters(value);
        }

        private class TypeWithDefaultConstructorThatThrows
        {
            public TypeWithDefaultConstructorThatThrows()
            {
                throw new Exception();
            }
        }

        private static class TypeThatCanNotBeInstantiated
        {
        }

        public class ClassWithDefaultConstructor
        {
        }

        public class TypeWithResolvableConstructorArguments<T1, T2>
        {
            public TypeWithResolvableConstructorArguments(T1 argument1, T2 argument2)
            {
            }
        }

        public class TypeWithCircularDependency
        {
            public TypeWithCircularDependency(TypeWithCircularDependency circular)
            {
            }
        }

        public class TypeWithMultipleConstructorsOfDifferentWidth
        {
            public bool WidestConstructorWasCalled;

            public TypeWithMultipleConstructorsOfDifferentWidth()
            {
            }

            public TypeWithMultipleConstructorsOfDifferentWidth(string argument1)
            {
            }

            public TypeWithMultipleConstructorsOfDifferentWidth(string argument1, string argument2)
            {
                this.WidestConstructorWasCalled = true;
            }
        }
    }
}
