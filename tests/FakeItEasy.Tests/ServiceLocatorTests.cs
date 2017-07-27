namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;
    using FluentAssertions;
    using Xunit;

    public class ServiceLocatorTests
    {
        public static IEnumerable<object[]> SingletonTypes()
        {
            return new[]
            {
                new object[] { typeof(IExpressionCallMatcherFactory) },
                new object[] { typeof(ExpressionArgumentConstraintFactory) },
                new object[] { typeof(IProxyGenerator) },
                new object[] { typeof(IFakeAndDummyManager) }
            };
        }

        public static IEnumerable<object[]> NonSingletonTypes()
        {
            return new[]
            {
                new object[] { typeof(IArgumentConstraintManagerFactory) },
                new object[] { typeof(FakeFacade) }
            };
        }

        [Fact]
        public void Current_should_not_be_null()
        {
            ServiceLocator.Current.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(SingletonTypes))]
        public void Should_be_registered_as_singleton(Type type)
        {
            var first = ServiceLocator.Current.Resolve(type);
            var second = ServiceLocator.Current.Resolve(type);

            second.Should().BeSameAs(first);
        }

        [Theory]
        [MemberData(nameof(NonSingletonTypes))]
        public void Should_be_registered_as_non_singleton(Type type)
        {
            var first = ServiceLocator.Current.Resolve(type);
            var second = ServiceLocator.Current.Resolve(type);

            second.Should().NotBeSameAs(first);
        }
    }
}
