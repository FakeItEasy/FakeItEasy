namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultSutInitializerTests
    {
        [Fake] private IFakeAndDummyManager fakeManager;
        private DefaultSutInitializer sutInitializer;

        private static IsFakeConstraint IsFake
        {
            get { return new IsFakeConstraint(); }
        }

        [SetUp]
        public void SetUp()
        {
            Fake.InitializeFixture(this);

            this.sutInitializer = new DefaultSutInitializer(this.fakeManager);
        }       

        [Test]
        public void Should_produce_sut_with_fakes_for_all_dependencies()
        {
            // Arrange
            this.StubFakeManagerWithFake<IFoo>();
            this.StubFakeManagerWithFake<object>();

            // Act
            var sut = (TypeWithFakeableDependencies)this.sutInitializer.CreateSut(typeof(TypeWithFakeableDependencies), (x, y) => { });
            
            // Assert
            Assert.That(sut.FooDependency, IsFake);
            Assert.That(sut.FooDependency2, IsFake);
            Assert.That(sut.ObjectDependency, IsFake);
        }

        [Test]
        public void Should_use_same_instance_when_multiple_arguments_are_of_same_type()
        {
            // Arrange
            this.StubFakeManagerWithFake<IFoo>();
            this.StubFakeManagerWithFake<object>();

            // Act
            var sut = (TypeWithFakeableDependencies)this.sutInitializer.CreateSut(typeof(TypeWithFakeableDependencies), (x, y) => { });

            // Assert
            Assert.That(sut.FooDependency, Is.SameAs(sut.FooDependency2));
        }

        [Test]
        public void Should_pass_empty_fake_options_to_fake_manager()
        {
            // Arrange
            this.StubFakeManagerWithFake<IFoo>();
            this.StubFakeManagerWithFake<object>();

            // Act
            var sut = (TypeWithFakeableDependencies)this.sutInitializer.CreateSut(typeof(TypeWithFakeableDependencies), (x, y) => { });

            // Assert
            A.CallTo(() => this.fakeManager.CreateFake(A<Type>._, A<FakeOptions>.That.Not.IsEmpty())).MustNotHaveHappened();
        }

        [Test]
        public void Should_call_callback_with_each_created_fake()
        {
            // Arrange
            var createdFakes = new Dictionary<Type, object>();

            this.StubFakeManagerWithFake<IFoo>();
            this.StubFakeManagerWithFake<object>();

            // Act
            var sut = (TypeWithFakeableDependencies)this.sutInitializer.CreateSut(typeof(TypeWithFakeableDependencies), createdFakes.Add);

            // Assert
            Assert.That(createdFakes[typeof(IFoo)], Is.SameAs(sut.FooDependency));
            Assert.That(createdFakes[typeof(object)], Is.SameAs(sut.ObjectDependency));
        }

        private void StubFakeManagerWithFake<T>()
        {
            A.CallTo(() => this.fakeManager.CreateFake(typeof(T), A<FakeOptions>._)).ReturnsLazily(() => A.Fake<T>());
        }

        public class TypeWithFakeableDependencies
        {
            public TypeWithFakeableDependencies()
            {
            }

            public TypeWithFakeableDependencies(
                IFoo fooDependency,
                IFoo fooDependency2, 
                object objectDependency)
            {
                this.FooDependency = fooDependency;
                this.FooDependency2 = fooDependency2;
                this.ObjectDependency = objectDependency;
            }

            public IFoo FooDependency { get; set; }

            public IFoo FooDependency2 { get; set; }

            public object ObjectDependency { get; set; }
        }
    }
}