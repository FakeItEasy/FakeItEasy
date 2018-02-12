namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using FluentAssertions.Specialized;
    using Xunit;

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed in teardown.")]
    public class NullGuardedAssertionTests
    {
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "text", Justification = "Required for testing.")]
        public static void UnguardedStaticMethod(string text)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "text", Justification = "Required for testing.")]
        public static void UnguardedMethodThatThrowsException(string text)
        {
            throw new InvalidOperationException();
        }

        [Fact]
        public void Assert_should_throw_when_call_is_null()
        {
            var exception = Record.Exception(() => ((Expression<Action>)null).Should().BeNullGuarded());
            exception.Should().BeAnExceptionOfType<ArgumentNullException>();
        }

        [Fact]
        public void Assert_should_fail_when_call_is_not_guarded()
        {
            AssertShouldFail(() => this.UnguardedMethod("foo"));
        }

        [Fact]
        public void Assert_should_pass_when_call_is_properly_guarded()
        {
            AssertShouldPass(() => this.GuardedMethod("foo"));
        }

        [Fact]
        public void Assert_should_pass_when_call_is_properly_guarded_constructor()
        {
#pragma warning disable CA1806 // Do not ignore method results
            AssertShouldPass(() => new ClassWithProperlyGuardedConstructor("foo"));
#pragma warning restore CA1806 // Do not ignore method results
        }

        [Fact]
        public void Assert_should_fail_when_argument_is_guarded_with_wrong_name()
        {
            AssertShouldFail(() => this.GuardedWithWrongName("foo"));
        }

        [Fact]
        public void Assert_should_pass_when_null_argument_is_valid_and_it_is_specified_as_null()
        {
            AssertShouldPass(() => this.FirstArgumentMayBeNull(null, "foo"));
        }

        [Fact]
        public void Assert_should_include_method_signature_in_error_message_when_call_is_non_guarded_constructor()
        {
#pragma warning disable CA1806 // Do not ignore method results
            AssertShouldFail(() => new ClassWithNonProperlyGuardedConstructor("foo", "bar")).And
#pragma warning restore CA1806 // Do not ignore method results
                .Message.Should().Contain("Expected calls to FakeItEasy.Tests.NullGuardedAssertionTests+ClassWithNonProperlyGuardedConstructor.ctor([String] a, [String] b) to be null guarded.");
        }

        [Fact]
        public void Assert_should_include_method_signature_in_error_message_when_call_is_non_guarded_method()
        {
            AssertShouldFail(() => this.UnguardedMethod("foo", "bar")).And
                .Message.Should().Contain("Expected calls to NullGuardedAssertionTests.UnguardedMethod([String] a, [String] b) to be null guarded.");
        }

        [Fact]
        public void Assert_should_include_call_signature_of_failing_calls_in_error_message_when_unguarded()
        {
            AssertShouldFail(() => this.UnguardedMethod("foo", "bar")).And.Message.Should()
                .Contain("(\"foo\", NULL) did not throw any exception.").And
                .Contain("(NULL, \"bar\") did not throw any exception.");
        }

        [Fact]
        public void Assert_should_include_call_signature_and_missing_argument_name_in_error_message_when_ArgumentNullException_was_thrown_with_wrong_name()
        {
            AssertShouldFail(() => this.GuardedWithWrongName("foo")).And.Message.Should()
                .Contain("(NULL) threw ArgumentNullException with wrong argument name, it should be a.");
        }

        [Fact]
        public void Assert_should_include_call_signature_and_exception_type_in_error_message_when_non_ArgumentNullException_was_thrown()
        {
            AssertShouldFail(() => UnguardedMethodThatThrowsException("foo")).And.Message.Should()
                .Contain("(NULL) threw unexpected System.InvalidOperationException.");
        }

        [Fact]
        public void Assert_should_be_callable_with_expression_that_has_value_types_in_the_parameter_list()
        {
            AssertShouldPass(() => this.GuardedMethodWithValueTypeArgument(1, "foo"));
        }

        [Fact]
        public void Assert_should_fail_when_nullable_argument_is_not_guarded()
        {
            AssertShouldFail(() => this.UnguardedMethodWithNullableArgument(1));
        }

        [Fact]
        public void Assert_should_pass_when_nullable_argument_is_guarded()
        {
            AssertShouldPass(() => this.GuardedMethodWithNullableArgument(1));
        }

        [Fact]
        public void Assert_should_be_callable_with_static_methods()
        {
            AssertShouldFail(() => UnguardedStaticMethod("foo"));
        }

        [Fact]
        public void Assert_should_fail_when_throwing_permutation_throws_unexpected_exception()
        {
            AssertShouldFail(() => UnguardedMethodThatThrowsException("foo"));
        }

        private static void AssertShouldPass(Expression<Action> call)
        {
            var exception = Record.Exception(() => call.Should().BeNullGuarded());
            exception.Should().BeNull();
        }

        private static ExceptionAssertions<AssertionFailedException> AssertShouldFail(Expression<Action> call)
        {
            var exception = Record.Exception(() => call.Should().BeNullGuarded());
            return exception.Should().BeAnExceptionOfType<AssertionFailedException>();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "a", Justification = "Required for testing.")]
        private void UnguardedMethod(string a)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "a", Justification = "Required for testing.")]
        private void UnguardedMethodWithNullableArgument(int? a)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "a", Justification = "Required for testing.")]
        private void GuardedMethodWithValueTypeArgument(int a, string b)
        {
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "a", Justification = "Required for testing.")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "b", Justification = "Required for testing.")]
        private void UnguardedMethod(string a, string b)
        {
        }

        private void GuardedMethod(string a)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
        }

        private void GuardedMethodWithNullableArgument(int? a)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Required for testing.")]
        private void GuardedWithWrongName(string a)
        {
            if (a == null)
            {
                throw new ArgumentNullException("b");
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "nullIsValid", Justification = "Required for testing.")]
        private void FirstArgumentMayBeNull(string nullIsValid, string nullIsNotValid)
        {
            if (nullIsNotValid == null)
            {
                throw new ArgumentNullException(nameof(nullIsNotValid));
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Created in an expression.")]
        private class ClassWithProperlyGuardedConstructor
        {
            public ClassWithProperlyGuardedConstructor(string a)
            {
                if (a == null)
                {
                    throw new ArgumentNullException(nameof(a));
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Created in an expression.")]
        private class ClassWithNonProperlyGuardedConstructor
        {
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "a", Justification = "Required for testing.")]
            public ClassWithNonProperlyGuardedConstructor(string a)
            {
            }

            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "a", Justification = "Required for testing.")]
            [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "b", Justification = "Required for testing.")]
            public ClassWithNonProperlyGuardedConstructor(string a, string b)
            {
            }
        }
    }
}
