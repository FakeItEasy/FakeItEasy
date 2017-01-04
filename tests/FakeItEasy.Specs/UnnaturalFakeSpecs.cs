namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class UnnaturalFakeSpecs
    {
        public interface IFoo
        {
            int Bar { get; set; }

            void VoidMethod();

            int MethodWithResult();
        }

        [Scenario]
        public static void CallsToVoidMethodInvokes(Fake<IFoo> fake, bool wasCalled)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And I configure a void method to invoke an action"
                .x(() => fake.CallsTo(f => f.VoidMethod()).Invokes(() => wasCalled = true));

            "When I call the method on the faked object"
                .x(() => fake.FakedObject.VoidMethod());

            "Then it invokes the action"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void CallsToVoidMethodThrows(
            Fake<IFoo> fake,
            Exception exception)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And I configure a void method to throw an exception"
                .x(() => fake.CallsTo(f => f.VoidMethod()).Throws<ArithmeticException>());

            "When I call the method on the faked object"
                .x(() => exception = Record.Exception(() => fake.FakedObject.VoidMethod()));

            "Then it throws the exception"
                .x(() => exception.Should().BeAnExceptionOfType<ArithmeticException>());
        }

        [Scenario]
        public static void CallsToVoidMethodCallsBaseMethod(
            Fake<AClass> fake)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<AClass>());

            "And I configure a void method to call the base method"
                .x(() => fake.CallsTo(f => f.VoidMethod()).CallsBaseMethod());

            "When I call the method on the faked object"
                .x(() => fake.FakedObject.VoidMethod());

            "Then it calls the base method"
                .x(() => fake.FakedObject.WasVoidMethodCalled.Should().BeTrue());
        }

        [Scenario]
        public static void CallsToVoidMethodDoesNothing(
            Fake<AClass> fake,
            Exception exception)
        {
            "Given a strict unnatural fake"
                .x(() => fake = new Fake<AClass>(options => options.Strict()));

            "And I configure a void method to do nothing"
                .x(() => fake.CallsTo(f => f.VoidMethod()).DoesNothing());

            "When I call the method on the faked object"
                .x(() => exception = Record.Exception(() => fake.FakedObject.VoidMethod()));

            "Then it doesn't throw"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void CallsToVoidMethodCallsBaseMethodAndDoesNothing(
            Fake<AClass> fake)
        {
            "Given a strict unnatural fake"
                .x(() => fake = new Fake<AClass>(options => options.Strict()));

            "And I configure a void method to call the base method"
                .x(() => fake.CallsTo(f => f.VoidMethod()).CallsBaseMethod());

            "And I configure the method to do nothing"
                .x(() => fake.CallsTo(f => f.VoidMethod()).DoesNothing());

            "When I call the method on the faked object"
                .x(() => fake.FakedObject.VoidMethod());

            "Then it does does nothing"
                .x(() => fake.FakedObject.WasVoidMethodCalled.Should().BeFalse());
        }

        [Scenario]
        public static void CallsToVoidMethodCallsBaseMethodAndDoesNothingAppliedToSameCallsTo(
            Fake<AClass> fake,
            IVoidArgumentValidationConfiguration callToVoidMethod,
            Exception exception)
        {
            "Given a strict unnatural fake"
                .x(() => fake = new Fake<AClass>(options => options.Strict()));

            "And I identify a void method to configure"
                .x(() => callToVoidMethod = fake.CallsTo(f => f.VoidMethod()));

            "And I configure the method to call the base method"
                .x(() => callToVoidMethod.CallsBaseMethod());

            "And I configure the method to do nothing using the same call specifier"
                .x(() => exception = Record.Exception(() => callToVoidMethod.DoesNothing()));

            "Then it throws an invalid operation exception"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidOperationException>());
        }

        [Scenario]
        public static void CallsToIntMethod(Fake<IFoo> fake, bool wasCalled)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And I configure an int-returning method to invoke an action"
                .x(() => fake.CallsTo(f => f.MethodWithResult()).Invokes(() => wasCalled = true));

            "When I call the method on the faked object"
                .x(() => fake.FakedObject.MethodWithResult());

            "Then it invokes the action"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void AnyCall(Fake<IFoo> fake, bool wasCalled)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And I configure any call to invoke an action"
                .x(() => fake.AnyCall().Invokes(() => wasCalled = true));

            "When I call a method on the faked object"
                .x(() => fake.FakedObject.MethodWithResult());

            "Then it invokes the action"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void CallsToVoidWithCallToWrongFake(Fake<IFoo> fake, IFoo wrong, Exception exception)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And an unrelated instance of the faked type"
                .x(() => wrong = A.Fake<IFoo>());

            "When I configure a fake with an expression that calls the wrong fake"
                .x(() => exception = Record.Exception(() => fake.CallsTo(f => wrong.VoidMethod()).DoesNothing()));

            "Then it throws an exception that describes the problem"
                .x(() => exception
                        .Should().BeAnExceptionOfType<ArgumentException>()
                        .And.Message.Should().Be("The target of this call is not the fake object being configured."));
        }

        [Scenario]
        public static void CallsToNonVoidWithCallToWrongFake(Fake<IFoo> fake, IFoo wrong, Exception exception)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And an unrelated instance of the faked type"
                .x(() => wrong = A.Fake<IFoo>());

            "When I configure a fake with an expression that calls the wrong fake"
                .x(() => exception = Record.Exception(() => fake.CallsTo(f => wrong.MethodWithResult()).Returns(42)));

            "Then it throws an exception that describes the problem"
                .x(() => exception
                        .Should().BeAnExceptionOfType<ArgumentException>()
                        .And.Message.Should().Be("The target of this call is not the fake object being configured."));
        }

        [Scenario]
        [Example(int.MinValue)]
        [Example(-42)]
        [Example(0)]
        [Example(42)]
        [Example(int.MaxValue)]
        public static void CallsToSetAnyValue(int value, Fake<IFoo> fake, bool wasCalled)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And assignment of a property is configured for any value"
                .x(() => fake.CallsToSet(f => f.Bar).Invokes(call => wasCalled = true));

            $"When I assign {value} to the property"
                .x(() => fake.FakedObject.Bar = value);

            "Then the configured behavior is used"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void CallsToSetSpecificValueAndAssigningThatValue(Fake<IFoo> fake, bool wasCalled)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And assignment of a property is configured for a specific value"
                .x(() => fake.CallsToSet(f => f.Bar).To(42).Invokes(call => wasCalled = true));

            "When I assign that value to the property"
                .x(() => fake.FakedObject.Bar = 42);

            "Then the configured behavior is used"
                .x(() => wasCalled.Should().BeTrue());
        }

        [Scenario]
        public static void CallsToSetSpecificValueAndAssigningDifferentValue(Fake<IFoo> fake, bool wasCalled)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And assignment of a property is configured for a specific value"
                .x(() => fake.CallsToSet(f => f.Bar).To(42).Invokes(call => wasCalled = true));

            "When I assign a different value to the property"
                .x(() => fake.FakedObject.Bar = 3);

            "Then the configured behavior is not used"
                .x(() => wasCalled.Should().BeFalse());
        }

        [Scenario]
        public static void CallsToSetWithCallToWrongFake(Fake<IFoo> fake, IFoo wrong, Exception exception)
        {
            "Given an unnatural fake"
                .x(() => fake = new Fake<IFoo>());

            "And an unrelated instance of the faked type"
                .x(() => wrong = A.Fake<IFoo>());

            "When I configure a fake with an expression that calls the wrong fake"
                .x(() => exception = Record.Exception(() => fake.CallsToSet(f => wrong.Bar).DoesNothing()));

            "Then it throws an exception that describes the problem"
                .x(() => exception
                        .Should().BeAnExceptionOfType<ArgumentException>()
                        .And.Message.Should().Be("The target of this call is not the fake object being configured."));
        }

        public class AClass
        {
            public bool WasVoidMethodCalled { get; private set; }

            public virtual void VoidMethod()
            {
                this.WasVoidMethodCalled = true;
            }
        }
    }
}
