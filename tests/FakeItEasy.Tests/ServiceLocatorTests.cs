namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceLocatorTests
    {
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private static IEnumerable<Type> SingletonTypes
        {
            get
            {
                yield return typeof(IExpressionCallMatcherFactory);
                yield return typeof(ExpressionArgumentConstraintFactory);
                yield return typeof(IProxyGenerator);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflectively.")]
        private static IEnumerable<Type> NonSingletonTypes
        {
            get
            {
                yield return typeof(IFakeCreatorFacade);
                yield return typeof(IFakeAndDummyManager);
                yield return typeof(IFixtureInitializer);
            }
        }

        [Test]
        public void Current_should_not_be_null()
        {
            ServiceLocator.Current.Should().NotBeNull();
        }

        [Test]
        public void Should_be_registered_as_singleton([ValueSource(nameof(SingletonTypes))] Type type)
        {
            var first = ServiceLocator.Current.Resolve(type);
            var second = ServiceLocator.Current.Resolve(type);

            second.Should().BeSameAs(first);
        }

        [Test]
        public void Should_be_registered_as_non_singleton([ValueSource(nameof(NonSingletonTypes))] Type type)
        {
            var first = ServiceLocator.Current.Resolve(type);
            var second = ServiceLocator.Current.Resolve(type);

            second.Should().NotBeSameAs(first);
        }
    }
}
