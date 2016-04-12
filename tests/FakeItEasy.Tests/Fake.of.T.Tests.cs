namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FluentAssertions;
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

            A.CallTo(() => this.fakeCreator.CreateFake(A<Action<IFakeOptions<IFoo>>>._)).Returns(foo);

            var fake = new Fake<IFoo>();

            fake.FakedObject.Should().BeSameAs(foo);
        }

        [Test]
        public void Constructor_that_takes_options_builder_should_be_null_guarded()
        {
            Action<IFakeOptions<Foo>> optionsBuilder = x => { };

            Expression<Action> call = () =>
                new Fake<Foo>(optionsBuilder);
            call.Should().BeNullGuarded();
        }

        [Test]
        public void Constructor_that_takes_options_builder_should_set_fake_returned_from_factory_to_FakedObject_property()
        {
            var argumentsForConstructor = new object[] { A.Fake<IFoo>() };
            var fakeReturnedFromFactory = A.Fake<AbstractTypeWithNoDefaultConstructor>(x => x.WithArgumentsForConstructor(argumentsForConstructor));

            Action<IFakeOptions<AbstractTypeWithNoDefaultConstructor>> optionsBuilder = x => { };

            A.CallTo(() => this.fakeCreator.CreateFake(optionsBuilder))
                .Returns(fakeReturnedFromFactory);

            var fake = new Fake<AbstractTypeWithNoDefaultConstructor>(optionsBuilder);

            fake.FakedObject.Should().BeSameAs(fakeReturnedFromFactory);
        }

        [Test]
        public void RecordedCalls_returns_recorded_calls_from_manager()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeManager(fake.FakedObject);

            fake.FakedObject.Bar();

            fake.RecordedCalls.Should().BeEquivalentTo(fakeObject.GetRecordedCalls());
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

            result.Should().BeSameAs(callConfig);
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

            result.Should().BeSameAs(callConfig);
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
            result.Should().BeSameAs(callConfig);
        }

        protected override void OnSetup()
        {
            this.fakeCreator = A.Fake<IFakeCreatorFacade>(x => x.Wrapping(ServiceLocator.Current.Resolve<IFakeCreatorFacade>()));
            this.startConfigurationFactory = A.Fake<IStartConfigurationFactory>(x => x.Wrapping(ServiceLocator.Current.Resolve<IStartConfigurationFactory>()));

            this.StubResolve(this.fakeCreator);
            this.StubResolve(this.startConfigurationFactory);
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
