namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class ConfigurationSpecs
    {
        public interface IFoo
        {
            void Bar();

            int Baz();

            string Bas();

            IFoo Bafoo();

            IFoo Bafoo(out int i);
        }

        [Scenario]
        public static void Callback(
            IFoo fake,
            bool wasCalled)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure a method to invoke an action"
                .x(() => A.CallTo(() => fake.Bar()).Invokes(x => wasCalled = true));

            "When I call the method"
                .x(() => fake.Bar());

            "Then it invokes the action"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void MultipleCallbacks(
            IFoo fake,
            bool firstWasCalled,
            bool secondWasCalled,
            int returnValue)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure a method to invoke two actions and return a value"
                .x(() =>
                    A.CallTo(() => fake.Baz())
                        .Invokes(x => firstWasCalled = true)
                        .Invokes(x => secondWasCalled = true)
                        .Returns(10));

            "When I call the method"
                .x(() => returnValue = fake.Baz());

            "Then it calls the first callback"
                .x(() => firstWasCalled.Should().BeTrue());

            "And it calls the first callback"
                .x(() => secondWasCalled.Should().BeTrue());

            "And it returns the configured value"
                .x(() => returnValue.Should().Be(10));
        }

        [Scenario]
        public static void CallBaseMethod(
            BaseClass fake,
            int returnValue,
            bool callbackWasInvoked)
        {
            "Given a fake"
                .x(() => fake = A.Fake<BaseClass>());

            "And I configure a method to invoke an action and call the base method"
                .x(() =>
                    A.CallTo(() => fake.ReturnSomething())
                        .Invokes(x => callbackWasInvoked = true)
                        .CallsBaseMethod());

            "When I call the method"
                .x(() => returnValue = fake.ReturnSomething());

            "Then it calls the base method"
                .x(() => fake.WasCalled.Should().BeTrue());

            "And it returns the value from base method"
                .x(() => returnValue.Should().Be(10));

            "And it invokes the callback"
                .x(() => callbackWasInvoked.Should().BeTrue());
        }

        [Scenario]
        public static void MultipleReturns(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure the return value for the method"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Returns(42);
                });

            "When I use the same configuration object to set the return value again"
                .x(() => exception = Record.Exception(() => configuration.Returns(0)));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void ReturnThenThrow(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure the return value for the method"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Returns(42);
                });

            "When I use the same configuration object to have the method throw an exception"
                .x(() => exception = Record.Exception(() => configuration.Throws<Exception>()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void ReturnThenCallsBaseMethod(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure the return value for the method"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Returns(42);
                });

            "When I use the same configuration object to have the method call the base method"
                .x(() => exception = Record.Exception(() => configuration.CallsBaseMethod()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void MultipleThrows(
            IFoo fake,
            IReturnValueArgumentValidationConfiguration<int> configuration,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure the return method to throw an exception"
                .x(() =>
                {
                    configuration = A.CallTo(() => fake.Baz());
                    configuration.Throws<ArgumentNullException>();
                });

            "When I use the same configuration object to have the method throw an exception again"
                .x(() => exception = Record.Exception(() => configuration.Throws<ArgumentException>()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void CallToObjectOnNonFake(
            BaseClass notAFake,
            Exception exception)
        {
            "Given an object that is not a fake"
                .x(() => notAFake = new BaseClass());

            "When I start to configure the object"
                .x(() => exception = Record.Exception(() => A.CallTo(notAFake)));

            "Then it throws an argument exception"
               .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                   .And.Message.Should().Contain("Object 'FakeItEasy.Specs.ConfigurationSpecs+BaseClass' of type FakeItEasy.Specs.ConfigurationSpecs+BaseClass is not recognized as a fake object."));
        }

        [Scenario]
        public static void CallToNonVirtualVoidOnNonFake(
            BaseClass notAFake,
            Exception exception)
        {
            "Given an object that is not a fake"
                .x(() => notAFake = new BaseClass());

            "When I start to configure a non-virtual void method on the object"
                .x(() => exception = Record.Exception(() => A.CallTo(() => notAFake.DoSomethingNonVirtual())));

            "Then it throws an argument exception"
               .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                   .And.Message.Should().Contain("Object 'FakeItEasy.Specs.ConfigurationSpecs+BaseClass' of type FakeItEasy.Specs.ConfigurationSpecs+BaseClass is not recognized as a fake object."));
        }

        [Scenario]
        public static void CallToNonVirtualVoidOnFake(
            BaseClass fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<BaseClass>());

            "When I start to configure a non-virtual void method on the fake"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.DoSomethingNonVirtual())));

            "Then it throws a fake configuration exception"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                    .And.Message.Should().Contain("Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."));
        }

        [Scenario]
        public static void CallToSealedVoidOnNonFake(
            DerivedClass notAFake,
            Exception exception)
        {
            "Given an object that is not a fake"
                .x(() => notAFake = new DerivedClass());

            "When I start to configure a sealed void method on the object"
                .x(() => exception = Record.Exception(() => A.CallTo(() => notAFake.DoSomething())));

            "Then it throws an argument exception"
               .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                   .And.Message.Should().Contain("Object 'FakeItEasy.Specs.ConfigurationSpecs+DerivedClass' of type FakeItEasy.Specs.ConfigurationSpecs+DerivedClass is not recognized as a fake object."));
        }

        [Scenario]
        public static void CallToSealedVoidOnFake(
            DerivedClass fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<DerivedClass>());

            "When I start to configure a sealed void method on the fake"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.DoSomething())));

            "Then it throws a fake configuration exception"
               .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                   .And.Message.Should().Contain("Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."));
        }

        [Scenario]
        public static void CallToNonVirtualNonVoidOnNonFake(
            BaseClass notAFake,
            Exception exception)
        {
            "Given an object that is not a fake"
                .x(() => notAFake = new BaseClass());

            "When I start to configure a non-virtual non-void method on the object"
                .x(() => exception = Record.Exception(() => A.CallTo(() => notAFake.ReturnSomethingNonVirtual())));

            "Then it throws an argument exception"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                    .And.Message.Should().Contain("Object 'FakeItEasy.Specs.ConfigurationSpecs+BaseClass' of type FakeItEasy.Specs.ConfigurationSpecs+BaseClass is not recognized as a fake object."));
        }

        [Scenario]
        public static void CallToNonVirtualNonVoidOnFake(
            BaseClass fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<BaseClass>());

            "When I start to configure a non-virtual non-void method on the fake"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.ReturnSomethingNonVirtual())));

            "Then it throws a fake configuration exception"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                    .And.Message.Should().Contain("Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."));
        }

        [Scenario]
        public static void CallToSealedNonVoidOnNonFake(
            DerivedClass notAFake,
            Exception exception)
        {
            "Given an object that is not a fake"
                .x(() => notAFake = new DerivedClass());

            "When I start to configure a sealed non-void method on the object"
                .x(() => exception = Record.Exception(() => A.CallTo(() => notAFake.ReturnSomething())));

            "Then it throws an argument exception"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                    .And.Message.Should().Contain("Object 'FakeItEasy.Specs.ConfigurationSpecs+DerivedClass' of type FakeItEasy.Specs.ConfigurationSpecs+DerivedClass is not recognized as a fake object."));
        }

        [Scenario]
        public static void CallToSealedNonVoidOnFake(
            DerivedClass fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<DerivedClass>());

            "When I start to configure a sealed non-void method on the fake"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.ReturnSomething())));

            "Then it throws a fake configuration exception"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                    .And.Message.Should().Contain("Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."));
        }

        [Scenario]
        public static void CallToSetNonVirtualOnNonFake(
            BaseClass notAFake,
            Exception exception)
        {
            "Given an object that is not a fake"
                .x(() => notAFake = new BaseClass());

            "When I start to configure a non-virtual property setter on the object"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => notAFake.SomeNonVirtualProperty)));

            "Then it throws an argument exception"
               .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                   .And.Message.Should().Contain("Object 'FakeItEasy.Specs.ConfigurationSpecs+BaseClass' of type FakeItEasy.Specs.ConfigurationSpecs+BaseClass is not recognized as a fake object."));
        }

        [Scenario]
        public static void CallToSetNonVirtualOnFake(
            BaseClass fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<BaseClass>());

            "When I start to configure a non-virtual property setter on the fake"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => fake.SomeNonVirtualProperty)));

            "Then it throws a fake configuration exception"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                    .And.Message.Should().Contain("Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."));
        }

        [Scenario]
        public static void CallToSetSealedOnNonFake(
            DerivedClass notAFake,
            Exception exception)
        {
            "Given an object that is not a fake"
                .x(() => notAFake = new DerivedClass());

            "When I start to configure a sealed property setter on the object"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => notAFake.SomeProperty)));

            "Then it throws an argument exception"
               .x(() => exception.Should().BeAnExceptionOfType<ArgumentException>()
                   .And.Message.Should().Contain("Object 'FakeItEasy.Specs.ConfigurationSpecs+DerivedClass' of type FakeItEasy.Specs.ConfigurationSpecs+DerivedClass is not recognized as a fake object."));
        }

        [Scenario]
        public static void CallToSetSealedOnFake(
            DerivedClass fake,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<DerivedClass>());

            "When I start to configure a sealed property setter on the fake"
                .x(() => exception = Record.Exception(() => A.CallToSet(() => fake.SomeProperty)));

            "Then it throws a fake configuration exception"
               .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                   .And.Message.Should().Contain("Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."));
        }

        [Scenario]
        public static void DoesNothingAfterStrictVoidDoesNotThrow(
            IFoo fake,
            Exception exception)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

            "And I configure a void method to do nothing"
                .x(() => A.CallTo(() => fake.Bar()).DoesNothing());

            "When I call the method"
                .x(() => exception = Record.Exception(() => fake.Bar()));

            "Then it does not throw an exception"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void DoesNothingAfterStrictValueTypeKeepsDefaultReturnValue(
            IFoo fake,
            int result)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

            "And I configure all methods to do nothing"
                .x(() => A.CallTo(fake).DoesNothing());

            "When I call a value type method"
                .x(() => result = fake.Baz());

            "Then it returns the same value as an unconfigured fake"
                .x(() => result.Should().Be(A.Fake<IFoo>().Baz()));
        }

        [Scenario]
        public static void DoesNothingAfterStrictNonFakeableReferenceTypeKeepsDefaultReturnValue(
            IFoo fake,
            string result)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

            "And I configure all methods to do nothing"
                .x(() => A.CallTo(fake).DoesNothing());

            "When I call a non-fakeable reference type method"
                .x(() => result = fake.Bas());

            "Then it returns the same value as an unconfigured fake"
                .x(() => result.Should().Be(A.Fake<IFoo>().Bas()));
        }

        [Scenario]
        public static void DoesNothingAfterStrictFakeableReferenceTypeKeepsDefaultReturnValue(
            IFoo fake,
            IFoo result)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

            "And I configure all methods to do nothing"
                .x(() => A.CallTo(fake).DoesNothing());

            "When I call a fakeable reference type method"
                .x(() => result = fake.Bafoo());

            "Then it returns the same value as an unconfigured fake"
                .x(() => result.Should().Be(A.Fake<IFoo>().Bafoo()));
        }

        [Scenario]
        public static void ThrowsAndDoesNothingAppliedToSameACallTo(
            IFoo fake,
            IVoidArgumentValidationConfiguration callToBar,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I identify a method to configure"
                .x(() => callToBar = A.CallTo(() => fake.Bar()));

            "And I configure the method to throw an exception"
                .x(() => callToBar.Throws<Exception>());

            "When I configure the method to do nothing"
                .x(() => exception = Record.Exception(() => callToBar.DoesNothing()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void DoesNothingAndThrowsAppliedToSameACallTo(
            IFoo fake,
            IVoidArgumentValidationConfiguration callToBar,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I identify a method to configure"
                .x(() => callToBar = A.CallTo(() => fake.Bar()));

            "And I configure the method to do nothing"
                .x(() => callToBar.DoesNothing());

            "When I configure the method to throw an exception"
                .x(() => exception = Record.Exception(() => callToBar.Throws<Exception>()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void CallsBaseMethodAndDoesNothing(BaseClass fake)
        {
            "Given a fake"
                .x(() => fake = A.Fake<BaseClass>());

            "And I configure a method to call the base method"
                .x(() => A.CallTo(() => fake.DoSomething()).CallsBaseMethod());

            "And I configure the method to do nothing"
                .x(() => A.CallTo(() => fake.DoSomething()).DoesNothing());

            "When I call the method"
                .x(() => fake.DoSomething());

            "Then it does nothing"
                .x(() => fake.WasCalled.Should().BeFalse());
        }

        [Scenario]
        public static void CallsBaseMethodAndDoesNothingAppliedToSameACallTo(
            BaseClass fake,
            IVoidArgumentValidationConfiguration callToDoSomething,
            Exception exception)
        {
            "Given a fake"
                .x(() => fake = A.Fake<BaseClass>());

            "And I identify a method to configure"
                .x(() => callToDoSomething = A.CallTo(() => fake.DoSomething()));

            "And I configure the method to call the base method"
                .x(() => callToDoSomething.CallsBaseMethod());

            "And I configure the method to do nothing"
                .x(() => exception = Record.Exception(() => callToDoSomething.DoesNothing()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void InvokesAfterStrictVoidDoesNotThrow(
            IFoo fake,
            Exception exception)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

            "And I configure a void method to invoke an action"
                .x(() => A.CallTo(() => fake.Bar()).Invokes(() => { }));

            "When I call the method"
                .x(() => exception = Record.Exception(() => fake.Bar()));

            "Then it does not throw an exception"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void InvokesAfterStrictValueTypeKeepsDefaultReturnValue(
            IFoo fake,
            int result)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

            "And I configure a value type method to invoke an action"
                .x(() => A.CallTo(() => fake.Baz()).Invokes(() => { }));

            "When I call the method"
                .x(() => result = fake.Baz());

            "Then it returns the same value as an unconfigured fake"
                .x(() => result.Should().Be(A.Fake<IFoo>().Baz()));
        }

        [Scenario]
        public static void InvokesAfterStrictNonFakeableReferenceTypeKeepsDefaultReturnValue(
            IFoo fake,
            string result)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

            "And I configure all methods to invoke an action"
                .x(() => A.CallTo(fake).Invokes(() => { }));

            "When I call a non-fakeable reference type method"
                .x(() => result = fake.Bas());

            "Then it returns the same value as an unconfigured fake"
                .x(() => result.Should().Be(A.Fake<IFoo>().Bas()));
        }

        [Scenario]
        public static void InvokesAfterStrictFakeableableReferenceTypeKeepsDefaultReturnValue(
            IFoo fake,
            IFoo result)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

            "And I configure all methods to invoke an action"
                .x(() => A.CallTo(fake).Invokes(() => { }));

            "When I call a fakeable reference type method"
                .x(() => result = fake.Bafoo());

            "Then it returns the same value as an unconfigured fake"
                .x(() => result.Should().Be(A.Fake<IFoo>().Bafoo()));
        }

        [Scenario]
        public static void AssignsOutAndRefParametersForAllMethodsKeepsDefaultReturnValue(
            IFoo fake,
            IFoo result,
            int i)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I configure all methods to assign out and ref parameters"
                .x(() => A.CallTo(fake).AssignsOutAndRefParameters(0));

            "When I call a reference type method"
                .x(() => result = fake.Bafoo(out i));

            "Then it returns the same value as an unconfigured fake"
                .x(() => result.Should().Be(A.Fake<IFoo>().Bafoo(out i)));
        }

        [Scenario]
        public static void UnusedVoidCallSpec(
            IFoo fake,
            Exception exception)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(o => o.Strict()));

            "When I specify a call to a void method without configuring its behavior"
                .x(() => A.CallTo(() => fake.Bar()));

            "And I make a call to that method"
                .x(() => exception = Record.Exception(() => fake.Bar()));

            "Then it throws an expectation exception"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public static void UnusedNonVoidCallSpec(
            IFoo fake,
            Exception exception)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(o => o.Strict()));

            "When I specify a call to a void method without configuring its behavior"
                .x(() => A.CallTo(() => fake.Baz()));

            "And I make a call to that method"
                .x(() => exception = Record.Exception(() => fake.Baz()));

            "Then it throws an expectation exception"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());
        }

        public class BaseClass
        {
            public bool WasCalled { get; private set; }

            public string SomeNonVirtualProperty { get; set; }

            public virtual string SomeProperty { get; set; }

            public virtual void DoSomething()
            {
                this.WasCalled = true;
            }

            public void DoSomethingNonVirtual()
            {
            }

            public virtual int ReturnSomething()
            {
                this.WasCalled = true;
                return 10;
            }

            public int ReturnSomethingNonVirtual()
            {
                return 11;
            }
        }

        public class DerivedClass : BaseClass
        {
            public sealed override string SomeProperty { get; set; }

            public sealed override void DoSomething()
            {
            }

            public sealed override int ReturnSomething()
            {
                return 10;
            }
        }

        public class Foo : IFoo
        {
            public void Bar()
            {
                throw new NotSupportedException();
            }

            public int Baz()
            {
                throw new NotSupportedException();
            }

            public string Bas()
            {
                throw new NotSupportedException();
            }

            public IFoo Bafoo()
            {
                throw new NotSupportedException();
            }

            public IFoo Bafoo(out int i)
            {
                throw new NotSupportedException();
            }
        }

        public class FooFactory : DummyFactory<IFoo>
        {
            public static IFoo Instance { get; } = new Foo();

            protected override IFoo Create()
            {
                return Instance;
            }
        }
    }
}
