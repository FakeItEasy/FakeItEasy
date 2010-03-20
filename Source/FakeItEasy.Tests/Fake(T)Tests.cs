using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Core;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;
using FakeItEasy.Tests.FakeConstraints;
using FakeItEasy.Core;
using System.Reflection;
using FakeItEasy.Expressions;
using System.Linq.Expressions;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeTTests
        : ConfigurableServiceLocatorTestBase
    {
        private FakeObjectFactory factory;
        private IStartConfigurationFactory startConfigurationFactory;

        protected override void OnSetUp()
        {
            this.factory = A.Fake<FakeObjectFactory>(x => x.Wrapping(ServiceLocator.Current.Resolve<FakeObjectFactory>()));
            this.startConfigurationFactory = A.Fake<IStartConfigurationFactory>(x => x.Wrapping(ServiceLocator.Current.Resolve<IStartConfigurationFactory>()));

            this.StubResolve<FakeObjectFactory>(this.factory);
            this.StubResolve<IStartConfigurationFactory>(this.startConfigurationFactory);
        }
        
        [Test]
        public void Constructor_sets_fake_object_returned_from_factory_to_FakedObject_property()
        {
            var foo = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                A.CallTo(() => this.factory.CreateFake(typeof(IFoo), null, false)).Returns(foo);

                var fake = new Fake<IFoo>();

                Assert.That(fake.FakedObject, Is.SameAs(foo));
            }
        }

        [Test]
        public void Constructor_that_takes_constructor_expression_sets_fake_object_returned_from_factory_to_FakedObject_property()
        {
            var foo = A.Fake<Foo>();
            var serviceProviderArgument = A.Fake<IServiceProvider>();

            using (Fake.CreateScope())
            {
                A.CallTo(() => this.factory.CreateFake(typeof(Foo), A<IEnumerable<object>>.That.IsThisSequence(new object[] { serviceProviderArgument }).Argument, false))
                    .Returns(foo);

                var fake = new Fake<Foo>(() => new Foo(serviceProviderArgument));

                Assert.That(fake.FakedObject, Is.SameAs(foo));
            }
        }

        [Test]
        public void Constructor_that_takes_constructor_expression_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new Fake<Foo>(() => new Foo(A.Fake<IServiceProvider>())));
        }

        [Test]
        public void Constructor_that_takes_constructor_expression_should_throw_if_the_specified_expression_is_not_a_constructor_call()
        {
            Assert.Throws<ArgumentException>(() =>
                new Fake<Foo>(() => CreateFoo()));
        }

        private static Foo CreateFoo()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Constructor_that_takes_wrapped_instance_should_create_fake_and_set_it_to_the_FakedObject_property()
        {
            var foo = A.Fake<IFoo>();

            var fake = new Fake<IFoo>(foo);

            Assert.That(fake.FakedObject, Is.InstanceOfType<IFakedProxy>());
        }

        [Test]
        public void Constructor_that_takes_wrapped_instance_should_create_fake_that_is_wrapper()
        {
            var fake = new Fake<IFoo>(A.Fake<IFoo>());

            Assert.That(fake.FakedObject, new WrappingFakeConstraint());
        }

        [Test]
        public void Constructor_that_takes_wrapped_instance_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new Fake<IFoo>(A.Fake<IFoo>()));
        }

        [Test]
        public void Constructor_that_takes_arguments_for_constructor_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new Fake<Foo>(new object[] { A.Fake<IServiceProvider>() }));
        }

        [Test]
        public void Constructor_that_takes_arguments_for_constructor_should_set_fake_returned_from_factory_to_FakedObject_property()
        {
            var argumentsForConstructor = new object[] { A.Fake<IFoo>() };
            var fakeReturnedFromFactory = A.Fake<AbstractTypeWithNoDefaultConstructor>(x => x.WithArgumentsForConstructor(argumentsForConstructor));

            using (Fake.CreateScope())
            {
                A.CallTo(() => this.factory.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), A<IEnumerable<object>>.That.IsThisSequence(argumentsForConstructor).Argument, false))
                    .Returns(fakeReturnedFromFactory);

                var fake = new Fake<AbstractTypeWithNoDefaultConstructor>(argumentsForConstructor);

                Assert.That(fake.FakedObject, Is.SameAs(fakeReturnedFromFactory));
            }
        }

        [Test]
        public void Constructor_that_takes_arguments_for_constructor_should_throw_when_faked_type_has_accessible_constructor()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new Fake<Foo>(new object[] { A.Fake<IServiceProvider>() }));
        }

        public abstract class AbstractTypeWithNoDefaultConstructor
        {
            protected AbstractTypeWithNoDefaultConstructor(IFoo foo)
            {

            }
        }

        [Test]
        public void AssertWasCalled_with_void_call_calls_WasCalled_on_assertions_from_factory()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);

            var factoryAssertions = A.Fake<IFakeAssertions<IFoo>>();
            
            Expression<Action<IFoo>> callSpecification = x => x.Bar();

            using (Fake.CreateScope())
            {
                var factory = this.StubResolveWithFake<IFakeAssertionsFactory>();
                A.CallTo(() => factory.CreateAsserter<IFoo>(fakeObject)).Returns(factoryAssertions);

                fake.AssertWasCalled(callSpecification);
            }

            Fake.Assert(factoryAssertions)
                .WasCalled(x => x.WasCalled(callSpecification));
        }

        [Test]
        public void AssertWasCalled_with_void_call_and_repeat_calls_WasCalled_on_assertions_from_factory()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);

            var factoryAssertions = A.Fake<IFakeAssertions<IFoo>>();

            Expression<Action<IFoo>> callSpecification = x => x.Bar();
            Expression<Func<int, bool>> repeat = x => true;

            using (Fake.CreateScope())
            {
                var factory = this.StubResolveWithFake<IFakeAssertionsFactory>();
                A.CallTo(() => factory.CreateAsserter<IFoo>(fakeObject)).Returns(factoryAssertions);

                fake.AssertWasCalled(callSpecification, repeat);
            }

            Fake.Assert(factoryAssertions)
                .WasCalled(x => x.WasCalled(callSpecification, repeat));
        }

        [Test]
        public void AssertWasCalled_with_function_call_calls_WasCalled_on_assertions_from_factory()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);
            
            var factoryAssertions = A.Fake<IFakeAssertions<IFoo>>();

            Expression<Func<IFoo, int>> callSpecification = x => x.Baz();
            
            using (Fake.CreateScope())
            {
                var factory = this.StubResolveWithFake<IFakeAssertionsFactory>();
                A.CallTo(() => factory.CreateAsserter<IFoo>(fakeObject))
                    .Returns(factoryAssertions);

                fake.AssertWasCalled(callSpecification);
            }

            foreach (var call in Fake.GetCalls(factoryAssertions))
            { 
                Console.WriteLine(call);
            }

            Fake.Assert(factoryAssertions)
                .WasCalled(x => x.WasCalled(callSpecification));
        }

        [Test]
        public void AssertWasCalled_with_function_call_and_repeat_calls_WasCalled_on_assertions_from_factory()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);

            var factoryAssertions = A.Fake<IFakeAssertions<IFoo>>();

            Expression<Func<IFoo, int>> callSpecification = x => x.Baz();
            Expression<Func<int, bool>> repeat = x => true;

            using (Fake.CreateScope())
            {
                var factory = this.StubResolveWithFake<IFakeAssertionsFactory>();
                A.CallTo(() => factory.CreateAsserter<IFoo>(fakeObject)).Returns(factoryAssertions);

                fake.AssertWasCalled(callSpecification, repeat);
            }

            Fake.Assert(factoryAssertions)
                .WasCalled(x => x.WasCalled(callSpecification, repeat));
        }

        [Test]
        public void RecordedCalls_returns_recorded_calls_in_scope()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);

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
            A.CallTo(() => this.startConfigurationFactory.CreateConfiguration<IFoo>(A<FakeObject>.That.Fakes(fake.FakedObject))).Returns(config);

            var result = fake.CallsTo(callSpecification);

            Assert.That(result, Is.SameAs(callConfig));
        }

        [Test]
        public void Calls_to_returns_fake_configuraion_for_the_faked_object_when_function_call_is_specified()
        {
            Expression<Func<IFoo, int>> callSpecification = x => x.Baz();

            var callConfig = A.Fake<IReturnValueArgumentValidationConfiguration<int>>();
            var config = A.Fake<IStartConfiguration<IFoo>>();
            A.CallTo(() => config.CallsTo(callSpecification)).Returns(callConfig);

            var fake = new Fake<IFoo>();
            A.CallTo(() => this.startConfigurationFactory.CreateConfiguration<IFoo>(A<FakeObject>.That.Fakes(fake.FakedObject))).Returns(config);

            var result = fake.CallsTo(callSpecification);

            Assert.That(result, Is.SameAs(callConfig));
        }

        [Test]
        public void AnyCall_returns_fake_configuration_for_the_faked_object()
        {
            // Arrange
            var fake = new Fake<IFoo>();

            var callConfig = A.Fake<IAnyCallConfiguration>();
            var config = A.Fake<IStartConfiguration<IFoo>>();

            A.CallTo(() => config.AnyCall()).Returns(callConfig);
            A.CallTo(() => this.startConfigurationFactory.CreateConfiguration<IFoo>(A<FakeObject>.That.Fakes(fake.FakedObject))).Returns(config);

            // Act
            var result = fake.AnyCall();

            // Assert
            Assert.That(result, Is.SameAs(callConfig));
        }
    }
}