namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using Xunit;

    public class FakeTTests
        : ConfigurableServiceLocatorTestBase
    {
        private readonly IStartConfigurationFactory startConfigurationFactory;

        public FakeTTests()
        {
            this.startConfigurationFactory = A.Fake<IStartConfigurationFactory>(x => x.Wrapping(ServiceLocator.Current.Resolve<IStartConfigurationFactory>()));

            this.StubResolve(this.startConfigurationFactory);
        }

        [Fact]
        public void Constructor_that_takes_options_builder_should_be_null_guarded()
        {
            Action<IFakeOptions<Foo>> optionsBuilder = x => { };

            Expression<Action> call = () =>
                new Fake<Foo>(optionsBuilder);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void RecordedCalls_returns_recorded_calls_from_manager()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeManager(fake.FakedObject);

            fake.FakedObject.Bar();

            fake.RecordedCalls.Should().BeEquivalentTo(fakeObject.GetRecordedCalls());
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        public abstract class AbstractTypeWithNoDefaultConstructor
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "foo", Justification = "Required for testing.")]
            protected AbstractTypeWithNoDefaultConstructor(IFoo foo)
            {
            }
        }
    }
}
