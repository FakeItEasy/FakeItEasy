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

        public static IEnumerable<object[]> NonInterceptableMembers()
        {
            return TestCases.FromObject(
                NonInterceptableTestCase.Create(
                    () => new object().GetType(),
                    "Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."),
                NonInterceptableTestCase.Create(
                    () => object.Equals("foo", "bar"),
                    "Static methods can not be intercepted."),
                NonInterceptableTestCase.Create(
                    () => "foo".Count(),
                    "Extension methods can not be intercepted since they're static."),
                NonInterceptableTestCase.Create(
                    () => new TypeWithSealedOverride().ToString(),
                    "Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."));
        }

        public static IEnumerable<object[]> InterceptableMethods()
        {
            return TestCases.FromObject(
                InterceptionTestCase.Create(() => new object().ToString()),
                InterceptionTestCase.Create(() =>
                    ((IBInterface)A.Fake<IAInterface>(builder => builder.Implements(typeof(IBInterface))))
                        .Method()));
        }

        [Theory]
        [MemberData(nameof(NonInterceptableMembers))]
        public void Should_fail_for_non_interceptable_methods(NonInterceptableTestCase testCase)
        {
            Guard.AgainstNull(testCase, nameof(testCase));

            // Arrange
            string reason;

            // Act
            var result = this.validator.MethodCanBeInterceptedOnInstance(testCase.Method, testCase.Target, out reason);

            // Assert
            result.Should().BeFalse();
            reason.Should().Be(testCase.FailReason);
        }

        [Theory]
        [MemberData(nameof(InterceptableMethods))]
        public void Should_succeed_for_interceptable_methods(InterceptionTestCase testCase)
        {
            Guard.AgainstNull(testCase, nameof(testCase));

            // Arrange
            string reason;

            // Act
            var result = this.validator.MethodCanBeInterceptedOnInstance(testCase.Method, testCase.Target, out reason);

            // Assert
            result.Should().BeTrue();
            reason.Should().BeNull();
        }

        public class InterceptionTestCase
        {
            public object Target { get; private set; }

            public MethodInfo Method { get; private set; }

            [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Needs to be callable with an expression.")]
            public static InterceptionTestCase Create(Expression<Action> expression)
            {
                return Create<InterceptionTestCase>(expression);
            }

            protected static T Create<T>(LambdaExpression expression) where T : InterceptionTestCase, new()
            {
                var parser = new CallExpressionParser();
                var parsed = parser.Parse(expression);

                return new T()
                {
                    Method = parsed.CalledMethod,
                    Target = parsed.CallTarget
                };
            }
        }

        public class NonInterceptableTestCase : InterceptionTestCase
        {
            public string FailReason { get; private set; }

            [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Needs to be callable with an expression.")]
            public static NonInterceptableTestCase Create(Expression<Action> expression, string failReason)
            {
                var result = InterceptionTestCase.Create<NonInterceptableTestCase>(expression);
                result.FailReason = failReason;
                return result;
            }
        }

        public class TypeWithSealedOverride
        {
            public sealed override string ToString()
            {
                return base.ToString();
            }
        }
    }
}
