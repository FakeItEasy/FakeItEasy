namespace FakeItEasy.Specs
{
    using System;
    using Machine.Specifications;

    public class when_trying_to_fake_invisible_internals
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => A.Fake<IInternal>());

        It should_throw_an_exception_with_a_message_containing_a_hint_at_using_internals_visible_to_attribute =
            () =>
            {
                exception.Message.ShouldContain("If either the type or constructor is internal, try adding the following attribute to the assembly:");
                exception.Message.ShouldContain("[assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2\")]");
            };

        internal interface IInternal
        {
        }
    }
}