namespace FakeItEasy.Tests.Creation.CastleDynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public interface IAInterface
    {
        string Method();
    }

    public interface IBInterface
    {
        string Method();
    }

    public class CastleDynamicProxyInterceptionValidatorTests
    {
        private readonly CastleDynamicProxyInterceptionValidator validator;

        public CastleDynamicProxyInterceptionValidatorTests()
        {
            this.validator = new CastleDynamicProxyInterceptionValidator(new MethodInfoManager());
        }

        public static IEnumerable<object?[]> NonInterceptableMembers()
        {
            return TestCases.FromObject(
                new NonInterceptableTestCase(
                    () => new object().GetType(),
                    "Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."),
                new NonInterceptableTestCase(
                    () => object.Equals("foo", "bar"),
                    "Static methods can not be intercepted."),
                new NonInterceptableTestCase(
                    #pragma warning disable CA1806 // Do not ignore method results
                    () => "foo".Distinct(),
                    #pragma warning restore CA1806 // Do not ignore method results
                    "Extension methods can not be intercepted since they're static."),
                new NonInterceptableTestCase(
                    () => new TypeWithSealedOverride().ToString(),
                    "Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."));
        }

        public static IEnumerable<object?[]> InterceptableMethods()
        {
            return TestCases.FromObject(
                new InterceptionTestCase(() => new object().ToString()),
                new InterceptionTestCase(() =>
                    ((IBInterface)A.Fake<IAInterface>(builder => builder.Implements(typeof(IBInterface))))
                        .Method()));
        }

        [Theory]
        [MemberData(nameof(NonInterceptableMembers))]
        public void Should_fail_for_non_interceptable_methods(NonInterceptableTestCase testCase)
        {
            Guard.AgainstNull(testCase);

            // Arrange

            // Act
            var result = this.validator.MethodCanBeInterceptedOnInstance(testCase.Method, testCase.Target, out var reason);

            // Assert
            result.Should().BeFalse();
            reason.Should().Be(testCase.FailReason);
        }

        [Theory]
        [MemberData(nameof(InterceptableMethods))]
        public void Should_succeed_for_interceptable_methods(InterceptionTestCase testCase)
        {
            Guard.AgainstNull(testCase);

            // Arrange

            // Act
            var result = this.validator.MethodCanBeInterceptedOnInstance(testCase.Method, testCase.Target, out var reason);

            // Assert
            result.Should().BeTrue();
            reason.Should().BeNull();
        }

        public class InterceptionTestCase
        {
            public InterceptionTestCase(Expression<Action> expression)
            {
                var parser = new CallExpressionParser();
                var parsed = parser.Parse(expression);

                this.Method = parsed.CalledMethod;
                this.Target = parsed.CallTarget;
            }

            public object? Target { get; }

            public MethodInfo Method { get; }
        }

        public class NonInterceptableTestCase : InterceptionTestCase
        {
            public NonInterceptableTestCase(Expression<Action> expression, string failReason)
                : base(expression)
            {
                this.FailReason = failReason;
            }

            public string FailReason { get; }
        }

        public class TypeWithSealedOverride
        {
            public sealed override string? ToString()
            {
                return base.ToString();
            }
        }
    }
}
