namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    internal interface IInternal
    {
    }

    public static class FakingInternalsSpecs
    {
        [Scenario]
        public static void InvisibleInternals(
            Exception exception)
        {
            "when trying to fake invisible internals"
                .x(() => exception = Record.Exception(() => A.Fake<IInternal>()));

            "it should throw an exception with a message containing a hint at using internals visible to attribute"
                .x(() =>
                    {
                        exception.Message.Should().Contain("Make it public, or internal and mark your assembly with");
                        exception.Message.Should().Contain("[assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2\")]");
                    });
        }

        [Scenario]
        public static void GenericTypeWithInternalTypeParameters(
            Exception exception)
        {
            "when trying to fake generic type with internal type parameters"
                .x(() => exception = Record.Exception(() => A.Fake<IList<IInternal>>()));

            "it should throw an exception with a message containing a hint at using internals visible to attribute"
                .x(() => exception.Message.Should()
                             .Contain(
                                 "No usable default constructor was found on the type System.Collections.Generic.IList`1[FakeItEasy.Specs.IInternal]")
                             .And.Contain(
                                 "because type FakeItEasy.Specs.IInternal is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2"));
        }

        [Scenario]
        public static void OverrideInternalMethod(
            TypeWithInternalMethod fake,
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<TypeWithInternalMethod>());

            "when trying to override internal method on type"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.InternalMethod()).Returns(17)));

            "it should throw an exception with a message complaining about accessibility"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                             .And.Message.Should().Contain("not accessible to DynamicProxyGenAssembly2"));
        }
    }

    public class TypeWithInternalMethod
    {
        internal virtual int InternalMethod()
        {
            return 8;
        }
    }
}
