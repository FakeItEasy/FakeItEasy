namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class WrappingFakeSpecs
    {
        public interface IFoo
        {
            int NonVoidMethod(string parameter);

            void VoidMethod(string parameter);

            void OutAndRefMethod(ref int @ref, out int @out);
        }

        [Scenario]
        public static void NonVoidSuccess(
            Foo realObject,
            IFoo wrapper,
            int result)
        {
            "Given a real object"
                .x(() => realObject = new Foo());

            "And a fake wrapping this object"
                .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

            "When a non-void method is called on the wrapper"
                .x(() => result = wrapper.NonVoidMethod("hello"));

            "Then the real object's method is called"
                .x(() => realObject.NonVoidMethodCalled.Should().BeTrue());

            "And the wrapper returns the value returned by the real object's method"
                .x(() => result.Should().Be(5));
        }

        [Scenario]
        public static void NonVoidException(
            Foo realObject,
            IFoo wrapper,
            Exception exception)
        {
            "Given a real object"
                .x(() => realObject = new Foo());

            "And a fake wrapping this object"
                .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

            "When a non-void method is called on the wrapper"
                .x(() => exception = Record.Exception(() => wrapper.NonVoidMethod(null)));

            "Then the real object's method is called"
                .x(() => realObject.NonVoidMethodCalled.Should().BeTrue());

            "And the wrapper throws the exception thrown by the real object's method"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>());

            "And the exception stack trace is preserved"
                .x(() => exception.StackTrace.Should().Contain("FakeItEasy.Specs.WrappingFakeSpecs.Foo.NonVoidMethod"));
        }

        [Scenario]
        public static void VoidSuccess(
            Foo realObject,
            IFoo wrapper)
        {
            "Given a real object"
                .x(() => realObject = new Foo());

            "And a fake wrapping this object"
                .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

            "When a void method is called on the wrapper"
                .x(() => wrapper.VoidMethod("hello"));

            "Then the real object's method is called"
                .x(() => realObject.VoidMethodCalled.Should().BeTrue());
        }

        [Scenario]
        public static void VoidException(
            Foo realObject,
            IFoo wrapper,
            Exception exception)
        {
            "Given a real object"
                .x(() => realObject = new Foo());

            "And a fake wrapping this object"
                .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

            "When a void method is called on the wrapper"
                .x(() => exception = Record.Exception(() => wrapper.VoidMethod(null)));

            "Then the real object's method is called"
                .x(() => realObject.VoidMethodCalled.Should().BeTrue());

            "And the wrapper throws the exception thrown by the real object's method"
                .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>());

            "And the exception stack trace is preserved"
                .x(() => exception.StackTrace.Should().Contain("FakeItEasy.Specs.WrappingFakeSpecs.Foo.VoidMethod"));
        }

        [Scenario]
        public static void OutAndRef(
            Foo realObject,
            IFoo wrapper,
            int @ref,
            int @out)
        {
            "Given a real object"
                .x(() => realObject = new Foo());

            "And a fake wrapping this object"
                .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

            "And a @ref variable with an initial value of 1"
                .x(() => @ref = 1);

            "When a method with out and ref parameters is called on the wrapper"
                .x(() => wrapper.OutAndRefMethod(ref @ref, out @out));

            "Then the real object's method is called"
                .x(() => realObject.OutAndRefMethodCalled.Should().BeTrue());

            "And the value of @ref is incremented"
                .x(() => @ref.Should().Be(2));

            "And the value of @out is set"
                .x(() => @out.Should().Be(42));
        }

        public class Foo : IFoo
        {
            public bool NonVoidMethodCalled { get; private set; }

            public bool VoidMethodCalled { get; private set; }

            public bool OutAndRefMethodCalled { get; private set; }

            public int NonVoidMethod(string parameter)
            {
                this.NonVoidMethodCalled = true;
                if (parameter == null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                return parameter.Length;
            }

            public void VoidMethod(string parameter)
            {
                this.VoidMethodCalled = true;
                if (parameter == null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }
            }

            public void OutAndRefMethod(ref int @ref, out int @out)
            {
                this.OutAndRefMethodCalled = true;
                @ref += 1;
                @out = 42;
            }
        }
    }
}
