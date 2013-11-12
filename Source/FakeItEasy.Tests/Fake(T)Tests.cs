namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class FakeTTests
        : ConfigurableServiceLocatorTestBase
    {
        private IFakeCreatorFacade fakeCreator;
        private IStartConfigurationFactory startConfigurationFactory;

        [Test]
        public void Constructor_sets_fake_object_returned_from_fake_creator_to_FakedObject_property()
        {
            var foo = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                A.CallTo(() => this.fakeCreator.CreateFake<IFoo>(A<Action<IFakeOptionsBuilder<IFoo>>>._)).Returns(foo);

                var fake = new Fake<IFoo>();

                Assert.That(fake.FakedObject, Is.SameAs(foo));
            }
        }

        [Test]
        public void Constructor_that_takes_options_should_be_null_guarded()
        {
            Action<IFakeOptionsBuilder<Foo>> options = x => { };

            NullGuardedConstraint.Assert(() =>
                new Fake<Foo>(options));
        }

        [Test]
        public void Constructor_that_takes_options_should_set_fake_returned_from_factory_to_FakedObject_property()
        {
            var argumentsForConstructor = new object[] { A.Fake<IFoo>() };
            var fakeReturnedFromFactory = A.Fake<AbstractTypeWithNoDefaultConstructor>(x => x.WithArgumentsForConstructor(argumentsForConstructor));

            Action<IFakeOptionsBuilder<AbstractTypeWithNoDefaultConstructor>> options = x => { };

            using (Fake.CreateScope())
            {
                A.CallTo(() => this.fakeCreator.CreateFake<AbstractTypeWithNoDefaultConstructor>(options))
                    .Returns(fakeReturnedFromFactory);

                var fake = new Fake<AbstractTypeWithNoDefaultConstructor>(options);

                Assert.That(fake.FakedObject, Is.SameAs(fakeReturnedFromFactory));
            }
        }

        [Test]
        public void RecordedCalls_returns_recorded_calls_in_scope()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeManager(fake.FakedObject);

            fake.FakedObject.Bar();

            Assert.That(fake.RecordedCalls, Is.EquivalentTo(fakeObject.RecordedCallsInScope));
        }

        [Test]
        public void Calls_to_returns_fake_configuration_for_the_faked_object_when_void_call_is_specified()
        {
            Expression<Action<IFoo>> callSpecification = x => x.Bar();

            var callConfig = A.Fake<IVoidArgumentValidationConfiguration>();
            var config = A.Fake<IStartConfiguration<IFoo>>();
            A.CallTo(() => config.CallsTo(callSpecification)).Returns(callConfig);

            var fake = new Fake<IFoo>();
            A.CallTo(() => this.startConfigurationFactory.CreateConfiguration<IFoo>(A<FakeManager>.That.Fakes(fake.FakedObject))).Returns(config);

            var result = fake.CallsTo(callSpecification);

            Assert.That(result, Is.SameAs(callConfig));
        }

        [Test]
        public void Calls_to_returns_fake_configuration_for_the_faked_object_when_function_call_is_specified()
        {
            Expression<Func<IFoo, int>> callSpecification = x => x.Baz();

            var callConfig = A.Fake<IReturnValueArgumentValidationConfiguration<int>>();
            var config = A.Fake<IStartConfiguration<IFoo>>();
            A.CallTo(() => config.CallsTo(callSpecification)).Returns(callConfig);

            var fake = new Fake<IFoo>();
            A.CallTo(() => this.startConfigurationFactory.CreateConfiguration<IFoo>(A<FakeManager>.That.Fakes(fake.FakedObject))).Returns(config);

            var result = fake.CallsTo(callSpecification);

            Assert.That(result, Is.SameAs(callConfig));
        }

        [Test]
        public void AnyCall_returns_fake_configuration_for_the_faked_object()
        {
            // Arrange
            var fake = new Fake<IFoo>();

            var callConfig = A.Fake<IAnyCallConfigurationWithNoReturnTypeSpecified>();
            var config = A.Fake<IStartConfiguration<IFoo>>();

            A.CallTo(() => config.AnyCall()).Returns(callConfig);
            A.CallTo(() => this.startConfigurationFactory.CreateConfiguration<IFoo>(A<FakeManager>.That.Fakes(fake.FakedObject))).Returns(config);

            // Act
            var result = fake.AnyCall();

            // Assert
            Assert.That(result, Is.SameAs(callConfig));
        }

        protected override void OnSetup()
        {
            this.fakeCreator = A.Fake<IFakeCreatorFacade>(x => x.Wrapping(ServiceLocator.Current.Resolve<IFakeCreatorFacade>()));
            this.startConfigurationFactory = A.Fake<IStartConfigurationFactory>(x => x.Wrapping(ServiceLocator.Current.Resolve<IStartConfigurationFactory>()));

            this.StubResolve<IFakeCreatorFacade>(this.fakeCreator);
            this.StubResolve<IStartConfigurationFactory>(this.startConfigurationFactory);
        }

        public abstract class AbstractTypeWithNoDefaultConstructor
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "foo", Justification = "Required for testing.")]
            protected AbstractTypeWithNoDefaultConstructor(IFoo foo)
            {
            }
        }
    }
}