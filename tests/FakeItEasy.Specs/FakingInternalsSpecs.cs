namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using LambdaTale;
    using Xunit;

    internal interface IInternal
    {
    }

    public static class FakingInternalsSpecs
    {
        [Scenario]
        public static void InvisibleInternalInterface(
            Exception exception)
        {
            "Given an internal interface not visible to DynamicProxyGenAssembly2"
                .See<IInternal>();

            "When I try to fake the interface"
                .x(() => exception = Record.Exception(() => A.Fake<IInternal>()));

            "Then it throws an exception with a message containing a hint at using internals visible to attribute"
                .x(() => exception.Message.Should()
                    .Contain("Make it public, or internal and mark your assembly with")
                    .And.Match("*[assembly: InternalsVisibleTo(*DynamicProxyGenAssembly2*)]*"));
        }

        [Scenario]
        public static void ClassWithInvisibleInternalConstructor(
            Exception exception)
        {
            "Given a public class with an internal constructor not visible to DynamicProxyGenAssembly2"
                .See<ClassWithInternalConstructor>();

            "When I try to fake the class"
                .x(() => exception = Record.Exception(() => A.Fake<ClassWithInternalConstructor>()));

            "Then it throws an exception with a message indicating that the constructor could not be found"
                .x(() => exception.Message.Should()
                    .ContainModuloLineEndings("Can not instantiate proxy of class: FakeItEasy.Specs.ClassWithInternalConstructor.\r\n      Could not find a parameterless constructor."));
        }

        [Scenario]
        public static void GenericTypeWithInternalTypeParameters(
            Exception exception)
        {
            "Given a public generic interface with a type parameter that is not visible to DynamicProxyGenAssembly2"
                .See<IList<IInternal>>();

            "When I try to fake the public interface"
                .x(() => exception = Record.Exception(() => A.Fake<IList<IInternal>>()));

            "Then it throws an exception with a message containing a hint at using internals visible to attribute"
                .x(() => exception.Message.Should()
                             .Contain(
                                 "No usable default constructor was found on the type System.Collections.Generic.IList`1[FakeItEasy.Specs.IInternal]")
                             .And.Match(
                                 "*because type FakeItEasy.Specs.IInternal is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(*DynamicProxyGenAssembly2*)]*"));
        }

        [Scenario]
        public static void OverrideInternalMethod(
            TypeWithInternalMethod fake,
            Exception exception)
        {
            const string expectedMessage = @"*The current proxy generator can not intercept the method FakeItEasy.Specs.TypeWithInternalMethod.InternalMethod() for the following reason:
    - Can not create proxy for method Int32 InternalMethod() because it or its declaring type is not accessible. Make it public, or internal and mark your assembly with [assembly: InternalsVisibleTo(*DynamicProxyGenAssembly2*)] attribute*";

            "Given a public type with an internal method not visible to DynamicProxyGenAssembly2"
                .See<TypeWithInternalMethod>();

            "And I create a fake of the type"
                .x(() => fake = A.Fake<TypeWithInternalMethod>());

            "When I override the internal method"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.InternalMethod()).Returns(17)));

            "Then it throws an exception with a message containing a hint at using internals visible to attribute"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                             .And.Message.Should().MatchModuloLineEndings(expectedMessage));
        }
    }

    public class ClassWithInternalConstructor
    {
        internal ClassWithInternalConstructor()
        {
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
