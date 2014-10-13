namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Machine.Specifications;

    public class ClassWhoseConstructorThrows
    {
        public ClassWhoseConstructorThrows()
        {
            throw new NotSupportedException("I don't like being constructed.");
        }
    }

    public class when_faking_a_class_whose_constructor_throws
    {
        private static Exception exception;

        Because of = () =>
            exception = Catch.Exception(() => A.Fake<ClassWhoseConstructorThrows>());

        It should_throw_a_FakeCreationException =
            () => exception.Should().BeOfType<FakeCreationException>();

        It should_throw_an_exception_whose_message_includes_original_exception_type =
            () => exception.Message.Should().Contain("of type System.NotSupportedException");

        It should_throw_an_exception_whose_message_includes_original_exception_message =
            () => exception.Message.Should().Contain("I don't like being constructed.");

        It should_throw_an_exception_whose_message_includes_original_exception_stack_trace =
            () => exception.Message.Should().Contain("at FakeItEasy.Specs.ClassWhoseConstructorThrows..ctor()");
    }
}