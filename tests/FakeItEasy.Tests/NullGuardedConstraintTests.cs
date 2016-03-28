namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed in teardown.")]
    [TestFixture]
    public class NullGuardedConstraintTests
    {
        private NullGuardedConstraint constraint;
        private MessageWriter writer;

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "text", Justification = "Required for testing.")]
        public static void UnguardedStaticMethod(string text)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "text", Justification = "Required for testing.")]
        public static void UnguardedMethodThatThrowsException(string text)
        {
            throw new InvalidOperationException();
        }

        [SetUp]
        public void Setup()
        {
            this.constraint = new NullGuardedConstraint();
            this.writer = new TextMessageWriter();
        }

        [TearDown]
        public void Teardown()
        {
            this.writer.Dispose();
        }

        [Test]
        public void Matches_should_throw_when_call_is_null()
        {
            var exception = Record.Exception(() => this.constraint.Matches(ToExpression(null)));
            exception.Should().BeAnExceptionOfType<ArgumentNullException>();
        }

        [Test]
        public void Matches_should_throw_when_actual_is_not_an_expression()
        {
            var exception = Record.Exception(() => this.constraint.Matches("not an expression"));
            exception.Should().BeAnExceptionOfType<ArgumentException>();
        }

        [Test]
        public void Matches_should_return_false_when_call_is_not_guarded()
        {
            this.constraint.Matches(ToExpression(() => this.UnguardedMethod("foo"))).Should().BeFalse();
        }

        [Test]
        public void Matches_should_return_true_when_call_is_properly_guarded()
        {
            this.constraint.Matches(ToExpression(() => this.GuardedMethod("foo"))).Should().BeTrue();
        }

        [Test]
        public void Matches_should_return_true_when_call_is_properly_guarded_constructor()
        {
            var expression = ToExpression(() => new ClassWithProperlyGuardedConstructor("foo"));
            this.constraint.Matches(expression).Should().BeTrue();
        }

        [Test]
        public void Matches_should_return_false_when_argument_is_guarded_with_wrong_name()
        {
            var call = ToExpression(() => this.GuardedWithWrongName("foo"));
            this.constraint.Matches(call).Should().BeFalse();
        }

        [Test]
        public void Matches_should_return_true_when_null_argument_is_valid_and_its_specified_as_null()
        {
            var call = ToExpression(() => this.FirstArgumentMayBeNull(null, "foo"));
            this.constraint.Matches(call).Should().BeTrue();
        }

        [Test]
        public void WriteMessageTo_should_write_method_signature_in_expected_message_when_call_is_non_guarded_constructor()
        {
            this.WriteConstraintMessageToWriter(() => new ClassWithNonProperlyGuardedConstructor("foo", "bar"));

            this.writer.ToString().Should().Contain(
                "Calls to FakeItEasy.Tests.NullGuardedConstraintTests+ClassWithNonProperlyGuardedConstructor.ctor([String] a, [String] b) should be null guarded.");
        }

        [Test]
        public void WriteMessageTo_should_write_method_signature_in_expected_message_when_call_is_non_guarded_method()
        {
            this.WriteConstraintMessageToWriter(() => this.UnguardedMethod("foo", "bar"));

            this.writer.ToString().Should()
                .Contain("Calls to NullGuardedConstraintTests.UnguardedMethod([String] a, [String] b) should be null guarded.");
        }

        [Test]
        public void WriteMessageTo_should_write_call_signature_of_failing_call_when_unguarded()
        {
            this.WriteConstraintMessageToWriter(() => this.UnguardedMethod("foo", "bar"));

            this.writer.ToString().Should()
                .Contain("(\"foo\", <NULL>) did not throw any exception.").And
                .Contain("(<NULL>, \"bar\") did not throw any exception.");
        }

        [Test]
        public void WriteMessageTo_should_write_call_signature_and_missing_argument_name_when_ArgumentNullException_was_thrown_with_wrong_name()
        {
            this.WriteConstraintMessageToWriter(() => this.GuardedWithWrongName("foo"));

            this.writer.ToString().Should()
                .Contain("(<NULL>) threw ArgumentNullException with wrong argument name, it should be \"a\".");
        }

        [Test]
        public void WriteMessageTo_should_write_call_signature_and_exception_type_when_non_ArgumentNullException_was_thrown()
        {
            this.WriteConstraintMessageToWriter(() => UnguardedMethodThatThrowsException("foo"));

            this.writer.ToString().Should()
                .Contain("(<NULL>) threw unexpected System.InvalidOperationException.");
        }

        [Test]
        public void Matches_should_be_callable_with_expression_that_has_value_types_in_the_parameter_list()
        {
            var call = ToExpression(() => this.GuardedMethodWithValueTypeArgument(1, "foo"));
            this.constraint.Matches(call).Should().BeTrue();
        }

        [Test]
        public void Matches_should_be_false_when_nullable_argument_is_not_guarded()
        {
            var call = ToExpression(() => this.UnguardedMethodWithNullableArgument(1));
            this.constraint.Matches(call).Should().BeFalse();
        }

        [Test]
        public void Matches_should_return_true_when_nullable_argument_is_guarded()
        {
            var call = ToExpression(() => this.GuardedMethodWithNullableArgument(1));
            this.constraint.Matches(call).Should().BeTrue();
        }

        [Test]
        public void Matches_should_be_callable_with_static_methods()
        {
            var call = ToExpression(() => UnguardedStaticMethod("foo"));
            this.constraint.Matches(call).Should().BeFalse();
        }

        [Test]
        public void Matches_should_return_false_when_throwing_permutation_throws_unexpected_exception()
        {
            var call = ToExpression(() => UnguardedMethodThatThrowsException("foo"));
            this.constraint.Matches(call).Should().BeFalse();
        }

        [Test]
        public void Assert_should_delegate_to_constraint()
        {
            var exception = Record.Exception(() => NullGuardedConstraint.Assert(() => this.UnguardedMethod("foo")));
            exception.Should().BeAnExceptionOfType<AssertionException>();
        }

        [Test]
        public void Assert_should_be_null_guarded()
        {
            var exception = Record.Exception(() => NullGuardedConstraint.Assert(null));
            exception.Should().BeAnExceptionOfType<ArgumentNullException>()
                .And.ParamName.Should().Be("call");
        }

        private static Expression<Action> ToExpression(Expression<Action> action)
        {
            return action;
        }

        private void CallMatchesOnConstraint(Expression<Action> action)
        {
            this.constraint.Matches(action);
        }

        private void WriteConstraintMessageToWriter(Expression<Action> action)
        {
            this.constraint.Matches(action);
            this.constraint.WriteMessageTo(this.writer);
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
                throw new ArgumentNullException("b");
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
                throw new ArgumentNullException("a");
            }
        }

        private void GuardedMethodWithNullableArgument(int? a)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
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
                throw new ArgumentNullException("nullIsNotValid");
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Created in an expression.")]
        private class ClassWithProperlyGuardedConstructor
        {
            public ClassWithProperlyGuardedConstructor(string a)
            {
                if (a == null)
                {
                    throw new ArgumentNullException("a");
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
