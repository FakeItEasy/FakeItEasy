namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceLocatorTests
    {
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private readonly Type[] singletonTypes =
        {
            typeof(IExpressionCallMatcherFactory),
            typeof(ExpressionArgumentConstraintFactory),
            typeof(IProxyGenerator)
        };

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflectively.")]
        private IEnumerable<Type> NonSingletonTypes
        {
            get { return Enumerable.Empty<Type>(); }
        }

        [Test]
        public void Current_should_not_be_null()
        {
            ServiceLocator.Current.Should().NotBeNull();
        }

        [Test]
        public void Should_be_registered_as_singleton([ValueSource("singletonTypes")] Type type)
        {
            var first = ServiceLocator.Current.Resolve(type);
            var second = ServiceLocator.Current.Resolve(type);

            second.Should().BeSameAs(first);
        }

        [Test]
        public void Should_be_registered_as_non_singleton([ValueSource("NonSingletonTypes")] Type type)
        {
            var first = ServiceLocator.Current.Resolve(type);
            var second = ServiceLocator.Current.Resolve(type);

            second.Should().NotBeSameAs(first);
        }
    }
}
