namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
            () => exception.Message.Should().Contain("FakeItEasy.Specs.ClassWhoseConstructorThrows..ctor()");
    }

    // This spec proves that we can cope with throwing constructors (e.g. ensures that FakeManagers won't be reused):
    public class when_faking_a_class_whose_first_constructor_fails
    {
        static FakedClass fake;

        Because of = () => fake = A.Fake<FakedClass>();

        It should_instantiate_the_fake_using_the_successful_constructor_with_the_longest_parameter_list = () =>
            fake.WasTwoParameterConstructorCalled.Should().BeTrue();

        It should_instantiate_a_fake_that_does_not_remember_the_failing_constructor_call = () =>
            fake.WasParameterlessConstructorCalled
                .Should().BeFalse("because the parameterless constructor was called for a different fake object");

        It should_only_have_tried_the_parameterless_constructor_and_one_with_the_longest_parameter_list = () =>
            FakedClass.ParameterListLengthsForAttemptedConstructors.Should().BeEquivalentTo(0, 2);

        public class FakedClass
        {
            private static ISet<int> parameterListLengthsForAttemptedConstructors = new SortedSet<int>();
            
            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            public FakedClass()
            {
                parameterListLengthsForAttemptedConstructors.Add(0);
                this.WasParameterlessConstructorCalled = true;

                throw new InvalidOperationException();
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someInterface", Justification = "This is just a dummy argument.")]
            public FakedClass(IDisposable someInterface)
            {
                parameterListLengthsForAttemptedConstructors.Add(1);
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someInterface", Justification = "This is just a dummy argument.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someName", Justification = "This is just a dummy argument.")]
            public FakedClass(IDisposable someInterface, string someName)
            {
                parameterListLengthsForAttemptedConstructors.Add(2);
                this.WasTwoParameterConstructorCalled = true;
            }

            public static ISet<int> ParameterListLengthsForAttemptedConstructors
            {
                get { return parameterListLengthsForAttemptedConstructors; }
            }

            public virtual bool WasParameterlessConstructorCalled { get; set; }

            public virtual bool WasTwoParameterConstructorCalled { get; set; }
        }
    }
}