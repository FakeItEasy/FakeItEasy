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
