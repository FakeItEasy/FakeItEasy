namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Machine.Specifications;

    public class when_faking_a_delegate_type
    {
        static Func<string, int> fakedDelegate;

        Establish context = () => fakedDelegate = A.Fake<Func<string, int>>();

        public class when_faking_a_delegate_type_and_invoking_without_configuration
        {
            Because of = () => fakedDelegate.Invoke("foo");

            It should_be_possible_to_assert_the_call = () => 
                A.CallTo(() => fakedDelegate.Invoke("foo")).MustHaveHappened();

            It should_be_possible_to_assert_the_call_without_specifying_invoke_method = () => 
                A.CallTo(() => fakedDelegate("foo")).MustHaveHappened();
        }

        public class when_faking_a_delegate_type_and_invoking_with_configuration
        {
            static int result;

            Establish context = () => A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Returns(10);

            Because of = () => result = fakedDelegate(null);

            It should_return_configured_value = () => result.Should().Be(10);
        }

        public class when_faking_a_delegate_type_and_invoking_with_throwing_configuration
        {
            static FormatException expectedException;
            static Exception exception;

            Establish context = () =>
            {
                expectedException = new FormatException();
                A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Throws(expectedException);
            };

            Because of = () => exception = Catch.Exception(() => fakedDelegate(null));

            It should_throw_the_configured_exception = () => exception.Should().BeSameAs(expectedException);
        }

        public class when_faking_a_delegate_type_and_invoking_with_configuration_without_specifying_invoke_method
        {
            static int result;

            Establish context = () => A.CallTo(() => fakedDelegate(A<string>._)).Returns(10);

            Because of = () => result = fakedDelegate(null);

            It should_return_configured_value = () => result.Should().Be(10);
        }
    }
}