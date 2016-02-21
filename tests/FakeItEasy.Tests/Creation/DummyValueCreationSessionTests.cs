namespace FakeItEasy.Tests.Creation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using NUnit.Framework;
    using Guard = FakeItEasy.Guard;

    [TestFixture]
    public class DummyValueCreationSessionTests
    {
        private IFakeObjectCreator fakeObjectCreator;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private object[] dummiesInContainer = new object[]
            {
                "dummy value",
                new Task<int>(() => 7),
                new Task(delegate { })
            };

        [SetUp]
        public void Setup()
        {
            this.fakeObjectCreator = A.Fake<IFakeObjectCreator>();
        }

        [TestCaseSource("dummiesInContainer")]
        public void Should_return_dummy_from_container_when_available(object dummyInContainer)
        {
            Guard.AgainstNull(dummyInContainer, "dummyInContainer");

            // Arrange
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakes(dummyInContainer),
                this.fakeObjectCreator);

            // Act
            object dummy;
            var result = session.TryResolveDummyValue(dummyInContainer.GetType(), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.SameAs(dummyInContainer));
        }

        [Test]
        public void Should_return_false_when_type_cannot_be_created()
        {
            // Arrange
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            object dummy = null;
            var result = session.TryResolveDummyValue(typeof(TypeThatCanNotBeInstantiated), out dummy);

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
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            object dummy;
            var result = session.TryResolveDummyValue(typeof(IFoo), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.SameAs(fake));
        }

        [Test]
        public void Should_return_default_value_when_type_is_value_type()
        {
            // Arrange
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            object dummy;
            var result = session.TryResolveDummyValue(typeof(int), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.EqualTo(0));
        }

        [Test]
        public void Should_be_able_to_create_class_with_default_constructor()
        {
            // Arrange
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            object dummy;
            var result = session.TryResolveDummyValue(typeof(ClassWithDefaultConstructor), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.InstanceOf<ClassWithDefaultConstructor>());
        }

        [Test]
        public void Should_be_able_to_create_class_with_resolvable_constructor_arguments()
        {
            // Arrange
            this.StubFakeObjectCreatorWithValue(A.Fake<IFoo>());
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakes("dummy string"),
                this.fakeObjectCreator);

            // Act
            object dummy;
            var result = session.TryResolveDummyValue(typeof(TypeWithResolvableConstructorArguments<string, IFoo>), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.InstanceOf<TypeWithResolvableConstructorArguments<string, IFoo>>());
        }

        [Test]
        public void Should_not_be_able_to_create_class_with_circular_dependencies()
        {
            // Arrange
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            object dummy;
            var result = session.TryResolveDummyValue(typeof(TypeWithCircularDependency), out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_be_able_to_resolve_same_type_twice_when_successful()
        {
            // Arrange
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakes("dummy value"),
                this.fakeObjectCreator);

            object dummy;
            session.TryResolveDummyValue(typeof(string), out dummy);
            dummy = null;

            // Act
            var result = session.TryResolveDummyValue(typeof(string), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.EqualTo("dummy value"));
        }

        [Test]
        public void Should_return_false_when_default_constructor_throws()
        {
            // Arrange
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            object dummy;
            var result = session.TryResolveDummyValue(typeof(TypeWithDefaultConstructorThatThrows), out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_favor_the_widest_constructor_when_activating()
        {
            // Arrange
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakes("dummy value"),
                this.fakeObjectCreator);

            // Act
            object dummy;
            session.TryResolveDummyValue(typeof(TypeWithMultipleConstructorsOfDifferentWidth), out dummy);
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
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            object dummy;
            var result = session.TryResolveDummyValue(restrictedType, out dummy);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Should_be_able_to_create_lazy_wrapper()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            this.StubFakeObjectCreatorWithValue<IFoo>(fake);
            var session = new DummyValueCreationSession(
                this.CreateDummyFactoryThatMakesNoDummy(),
                this.fakeObjectCreator);

            // Act
            object dummy;
            var result = session.TryResolveDummyValue(typeof(Lazy<IFoo>), out dummy);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dummy, Is.InstanceOf<Lazy<IFoo>>());
            Assert.That(((Lazy<IFoo>)dummy).Value, Is.SameAs(fake));
        }

        private void StubFakeObjectCreatorWithValue<T>(T value)
        {
            object output;
            A.CallTo(() => this.fakeObjectCreator.TryCreateFakeObject(typeof(T), A<DummyValueCreationSession>._, out output))
                .Returns(true)
                .AssignsOutAndRefParameters(value);
        }

        private DynamicDummyFactory CreateDummyFactoryThatMakes(object value)
        {
            return new DynamicDummyFactory(new[] { new FixedDummyFactory(value) });
        }

        private DynamicDummyFactory CreateDummyFactoryThatMakesNoDummy()
        {
            return new DynamicDummyFactory(new IDummyFactory[0]);
        }

        public class ClassWithDefaultConstructor
        {
        }

        public class TypeWithResolvableConstructorArguments<T1, T2>
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument2", Justification = "Required for testing.")]
            public TypeWithResolvableConstructorArguments(T1 argument1, T2 argument2)
            {
            }
        }

        public class TypeWithCircularDependency
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "circular", Justification = "Required for testing.")]
            public TypeWithCircularDependency(TypeWithCircularDependency circular)
            {
            }
        }

        public class TypeWithMultipleConstructorsOfDifferentWidth
        {
            public TypeWithMultipleConstructorsOfDifferentWidth()
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            public TypeWithMultipleConstructorsOfDifferentWidth(string argument1)
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument1", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "argument2", Justification = "Required for testing.")]
            public TypeWithMultipleConstructorsOfDifferentWidth(string argument1, string argument2)
            {
                this.WidestConstructorWasCalled = true;
            }

            public bool WidestConstructorWasCalled { get; set; }
        }

        internal class FixedDummyFactory : IDummyFactory
        {
            private readonly object dummy;

            public FixedDummyFactory(object dummy)
            {
                this.dummy = dummy;
            }

            public Priority Priority
            {
                get { return Priority.Default; }
            }

            public bool CanCreate(Type type)
            {
                return type == this.dummy.GetType();
            }

            public object Create(Type type)
            {
                return this.dummy;
            }
        }

        private static class TypeThatCanNotBeInstantiated
        {
        }

        private class TypeWithDefaultConstructorThatThrows
        {
            public TypeWithDefaultConstructorThatThrows()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
