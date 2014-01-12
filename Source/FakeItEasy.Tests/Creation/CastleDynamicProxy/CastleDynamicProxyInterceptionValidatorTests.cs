namespace FakeItEasy.Tests.Creation.CastleDynamicProxy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using FakeItEasy.Expressions;
    using NUnit.Framework;
    using Guard = FakeItEasy.Guard;

    public interface IAInterface
    {
        string Method();
    }

    public interface IBInterface
    {
        string Method();
    }

    [TestFixture]
    public class CastleDynamicProxyInterceptionValidatorTests
    {
        private CastleDynamicProxyInterceptionValidator validator;
        private MethodInfoManager methodInfoManager;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private object[] nonInterceptableMembers = new[]
        {
            NonInterceptableTestCase.Create(() => new object().GetType(), "Non virtual methods can not be intercepted."),
            NonInterceptableTestCase.Create(() => object.Equals("foo", "bar"), "Static methods can not be intercepted."),
            NonInterceptableTestCase.Create(() => "foo".Count(), "Extension methods can not be intercepted since they're static."),
            NonInterceptableTestCase.Create(() => new TypeWithSealedOverride().ToString(), "Sealed methods can not be intercepted.")
        };

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private InterceptionTestCase[] interceptableMethods = new[]
        {
            InterceptionTestCase.Create(() => new object().ToString()),
            InterceptionTestCase.Create(() => ((IBInterface)A.Fake<IAInterface>(builder => builder.Implements(typeof(IBInterface)))).Method())
        };

        [SetUp]
        public void Setup()
        {
            this.methodInfoManager = new MethodInfoManager();

            this.validator = new CastleDynamicProxyInterceptionValidator(this.methodInfoManager);
        }

        [TestCaseSource("nonInterceptableMembers")]
        public void Should_fail_for_non_interceptable_methods(NonInterceptableTestCase testCase)
        {
            Guard.AgainstNull(testCase, "testCase");

            // Arrange
            string reason = null;

            // Act
            var result = this.validator.MethodCanBeInterceptedOnInstance(testCase.Method, testCase.Target, out reason);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(reason, Is.EqualTo(testCase.FailReason));
        }

        [TestCaseSource("interceptableMethods")]
        public void Should_succeed_for_interceptable_methods(InterceptionTestCase testCase)
        {
            Guard.AgainstNull(testCase, "testCase");

            // Arrange
            string reason = null;

            // Act
            var result = this.validator.MethodCanBeInterceptedOnInstance(testCase.Method, testCase.Target, out reason);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(reason, Is.Null);
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