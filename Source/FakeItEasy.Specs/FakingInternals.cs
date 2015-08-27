namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

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

    public class FakingInternals
    {
        [Scenario]
        public void when_trying_to_fake_invisible_internals(
            Exception exception)
        {
            "when trying to fake invisible internals"
                .x(() => exception = Catch.Exception(() => A.Fake<IInternal>()));

            "it should throw an exception with a message containing a hint at using internals visible to attribute"
                .x(() =>
                    {
                        exception.Message.Should().Contain("Make it public, or internal and mark your assembly with");
                        exception.Message.Should().Contain("[assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2\")]");
                    });
        }
   
        [Scenario]
        public void when_trying_to_fake_generic_type_with_internal_type_parameters(
            Exception exception)
        {
            "when trying to fake invisible internals"
                .x(() => exception = Catch.Exception(() => A.Fake<IList<IInternal>>()));

            "it should throw an exception with a message containing a hint at using internals visible to attribute"
                .x(() => exception.Message.Should()
                             .Contain(
                                 "No usable default constructor was found on the type System.Collections.Generic.IList`1[FakeItEasy.Specs.IInternal]")
                             .And.Contain(
                                 "because type FakeItEasy.Specs.IInternal is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2"));
        }
   
        [Scenario]
        public void when_trying_to_override_internal_method_on_type(
            TypeWithInternalMethod fake, 
            Exception exception)
        {
            "establish"
                .x(() => fake = A.Fake<TypeWithInternalMethod>());

            "when trying to fake invisible internals"
                .x(() => exception = Catch.Exception(() => A.CallTo(() => fake.InternalMethod()).Returns(17)));

            "it should throw an exception with a message complaining about accessibility"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                             .And.Message.Should().Contain("not accessible to DynamicProxyGenAssembly2"));
        }
    }
}