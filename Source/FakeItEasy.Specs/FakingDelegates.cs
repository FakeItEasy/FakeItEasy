namespace FakeItEasy.IntegrationTests
{
    using System;
    using Machine.Specifications;

    public class when_faking_a_delegate_type
    {
        static Func<string, int> fakedDelegate;

        Because of = () => fakedDelegate = A.Fake<Func<string, int>>();

        It should_be_able_to_intercept_call = () =>
            {
                fakedDelegate.Invoke("foo");
                A.CallTo(() => fakedDelegate.Invoke("foo")).MustHaveHappened();
            };

        It should_be_able_to_set_return_value = () =>
            {
                A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Returns(10);
                fakedDelegate(null).ShouldEqual(10);
            };

        It should_be_able_to_configure_delegate_to_throw = () =>
            {
                var expectedException = new FormatException();

                A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Throws(expectedException);

                typeof (FormatException).ShouldBeThrownBy(() => fakedDelegate(null));
            };

        It should_be_able_to_configure_delegate_without_specifying_invokes_method = () =>
            {
                A.CallTo(() => fakedDelegate(A<string>._)).Returns(10);
                fakedDelegate(null).ShouldEqual(10);
            };
    }
}