namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Machine.Specifications;

    internal interface IInternal
    {
    }

    public class TypeWithInternalMethod
    {
        internal virtual int InternalMethod()
        {
            return 8;
        }
    }

    public class when_trying_to_fake_invisible_internals
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => A.Fake<IInternal>());

        It should_throw_an_exception_with_a_message_containing_a_hint_at_using_internals_visible_to_attribute =
            () =>
            {
                exception.Message.Should().Contain("Make it public, or internal and mark your assembly with");
                exception.Message.Should().Contain("[assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2\")]");
            };
    }

    public class when_trying_to_fake_generic_type_with_internal_type_parameters
    {
        private static Exception exception;

        Because of = () => exception = Catch.Exception(() => A.Fake<IList<IInternal>>());

        It should_throw_an_exception_with_a_message_containing_a_hint_at_using_internals_visible_to_attribute =
            () => exception.Message.Should()
                .Contain(
                    "No usable default constructor was found on the type System.Collections.Generic.IList`1[FakeItEasy.Specs.IInternal]")
                .And.Contain(
                    "because type FakeItEasy.Specs.IInternal is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2");
    }

    public class when_trying_to_override_internal_method_on_type
    {
        private static TypeWithInternalMethod fake;
        private static Exception exception;

        private Establish context = () => fake = A.Fake<TypeWithInternalMethod>();

        private Because of = () => exception = Catch.Exception(() => A.CallTo(() => fake.InternalMethod()).Returns(17));

        private It should_throw_an_exception_with_a_message_complaining_about_accessibility =
            () =>
                exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                    .And.Message.Should().Contain("not accessible to DynamicProxyGenAssembly2");
    }
}