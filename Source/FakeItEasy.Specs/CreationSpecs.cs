namespace FakeItEasy.Specs
{
    using System;
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
        static Action<FakedClass> onFakeConfiguration;
        static FakedClass fake;

        Establish context = () =>
        {
            onFakeConfiguration = A.Fake<Action<FakedClass>>();
        };

        Because of = () => fake = A.Fake<FakedClass>(options => options.ConfigureFake(onFakeConfiguration));

        It should_instantiate_the_fake_using_the_second_constructor = () =>
            fake.SecondConstructorCalled.Should().BeTrue();

        It should_use_a_fake_manager_which_did_not_receive_the_first_constructor_call = () =>
            fake.DefaultConstructorCalled.Should().BeFalse("because the default constructor was called on a *different* fake object");

        It should_call_fake_configuration_actions_for_each_constructor = () =>
            A.CallTo(() => onFakeConfiguration(A<FakedClass>._)).MustHaveHappened(Repeated.Exactly.Twice);

        public class FakedClass
        {
            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            public FakedClass()
            {
                this.DefaultConstructorCalled = true;

                throw new InvalidOperationException();
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This anti-pattern is part of the the tested scenario.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "someInterface", Justification = "This is just a dummy argument.")]
            public FakedClass(IDisposable someInterface)
            {
                this.SecondConstructorCalled = true;
            }

            public virtual bool DefaultConstructorCalled { get; set; }

            public virtual bool SecondConstructorCalled { get; set; }
        }
    }
}