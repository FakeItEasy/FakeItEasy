using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Configuration;
using FakeItEasy.Core;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;
using Castle.Core.Interceptor;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class NullGuardedConstraintTests
    {
        NullGuardedConstraint constraint;
        MessageWriter writer;
        
        [SetUp]
        public void SetUp()
        {
            this.constraint = new NullGuardedConstraint();
            this.writer = new TextMessageWriter();
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

        [Test]
        public void Matches_should_throw_when_call_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => constraint.Matches(ToExpression(null)));
        }

        [Test]
        public void Matches_should_throw_when_actual_is_not_an_expression()
        {
            Assert.Throws<ArgumentException>(() =>
                constraint.Matches("not an expression"));
        }

        [Test]
        public void Matches_should_return_false_when_call_is_not_guarded()
        {
            Assert.That(constraint.Matches(ToExpression(() => UnguardedMethod("foo"))), Is.False);
        }

        [Test]
        public void Matches_should_return_true_when_call_is_properly_guarded()
        {
            Assert.That(constraint.Matches(ToExpression(() => GuardedMethod("foo"))), Is.True);
        }

        [Test]
        public void Matches_should_return_true_when_call_is_properly_guarded_constructor()
        {
            var expression = ToExpression(() => new ClassWithProperlyGuardedConstructor("foo"));
            Assert.That(constraint.Matches(expression), Is.True);
        }

        [Test]
        public void Matches_should_return_false_when_argument_is_guarded_with_wrong_name()
        {
            var call = ToExpression(() => GuardedWithWrongName("foo"));
            Assert.That(constraint.Matches(call), Is.False);
        }

        [Test]
        public void Matches_should_return_ture_when_null_argument_is_valid_and_its_specified_as_null()
        {
            var call = ToExpression(() => FirstArgumentMayBeNull(null, "foo"));
            Assert.That(constraint.Matches(call), Is.True);
        }

        [Test]
        public void WriteMessageTo_should_write_method_signature_in_expected_message_when_call_is_non_guarded_constructor()
        {
            WriteConstraintMessageToWriter(() => new ClassWithNonProperlyGuardedConstructor("foo", "bar"));

            Assert.That(this.writer.ToString(),
                Text.Contains("Calls to FakeItEasy.Tests.NullGuardedConstraintTests+ClassWithNonProperlyGuardedConstructor.ctor([String] a, [String] b) should be null guarded."));
        }

        [Test]
        public void WriteMessageTo_should_write_method_signature_in_expected_message_when_call_is_non_guarded_method()
        {
            WriteConstraintMessageToWriter(() => UnguardedMethod("foo", "bar"));

            Assert.That(this.writer.ToString(),
                Text.Contains("Calls to NullGuardedConstraintTests.UnguardedMethod([String] a, [String] b) should be null guarded."));
        }

        [Test]
        public void WriteMessageTo_should_write_call_signature_of_failing_call_when_unguarded()
        {
            WriteConstraintMessageToWriter(() => UnguardedMethod("foo", "bar"));

            Assert.That(this.writer.ToString(),
                Text.Contains("(\"foo\", <NULL>) did not throw any exception.")
                .And.Contains("(<NULL>, \"bar\") did not throw any exception."));
        }

        [Test]
        public void WriteMessageTo_should_write_call_signature_and_missing_argument_name_when_ArgumentNullException_was_thrown_with_wrong_name()
        {
            WriteConstraintMessageToWriter(() => GuardedWithWrongName("foo"));

            Assert.That(this.writer.ToString(),
                Text.Contains("(<NULL>) threw ArgumentNullException with wrong argument name, it should be \"a\"."));
        }

        [Test]
        public void WriteMessageTo_should_write_call_signature_and_exception_type_when_non_ArgumentNullException_was_thrown()
        {
            WriteConstraintMessageToWriter(() => UnguardedMethodThatThrowsException("foo"));

            Assert.That(this.writer.ToString(),
                Text.Contains("(<NULL>) threw unexpected System.ApplicationException."));
        }

        [Test]
        public void Matches_should_be_callable_with_expression_that_has_value_types_in_the_parameter_list()
        {
            var call = ToExpression(() => GuardedMethodWithValueTypeArgument(1, "foo"));
            Assert.That(constraint.Matches(call), Is.True);
        }

        [Test]
        public void Matches_should_be_false_when_nullable_argument_is_not_guarded()
        {
            var call = ToExpression(() =>  UnguardedMethodWithNullableArgument(1));
            Assert.That(constraint.Matches(call), Is.False);
        }

        [Test]
        public void Matches_should_return_true_when_nullable_argument_is_guarded()
        {
            var call = ToExpression(() => GuardedMethodWithNullableArgument(1));
            Assert.That(constraint.Matches(call), Is.True);
        }

        [Test]
        public void Matches_should_be_callable_with_static_methods()
        {
            var call = ToExpression(() => UnguardedStaticMethod("foo"));
            Assert.That(constraint.Matches(call), Is.False);
        }

        [Test]
        public void Mathces_should_return_false_when_throwing_permutation_throws_unexpected_exception()
        {
            var call = ToExpression(() => UnguardedMethodThatThrowsException("foo"));
            Assert.That(constraint.Matches(call), Is.False);
        }

        [Test]
        public void Assert_should_delegate_to_constraint()
        {
            Assert.Throws<AssertionException>(() =>
                NullGuardedConstraint.Assert(() => UnguardedMethod("foo")));

        }

        [Test]
        public void Assert_should_be_null_guarded()
        {
            try
            {
                NullGuardedConstraint.Assert(null);
                Assert.Fail("Exception should be thrown.");
            }
            catch (ArgumentNullException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("call"));
            }
        }

        private class ClassWithProperlyGuardedConstructor
        {
            public ClassWithProperlyGuardedConstructor(string a)
            {
                if (a == null) throw new ArgumentNullException("a");
            }
        }

        private class ClassWithNonProperlyGuardedConstructor
        {
            public ClassWithNonProperlyGuardedConstructor(string a)
            { }

            public ClassWithNonProperlyGuardedConstructor(string a, string b)
            { }
        }

        public static void UnguardedStaticMethod(string a)
        {

        }

        public static void UnguardedMethodThatThrowsException(string a)
        {
            throw new ApplicationException();
        }

        private void UnguardedMethod(string a)
        {

        }

        private void UnguardedMethodWithNullableArgument(int? a)
        {

        }

        private void GuardedMethodWithValueTypeArgument(int a, string b)
        {
            if (b == null) throw new ArgumentNullException("b");
        }

        private void UnguardedMethod(string a, string b)
        {

        }

        private void GuardedMethod(string a)
        {
            if (a == null) throw new ArgumentNullException("a");
        }

        private void GuardedMethodWithNullableArgument(int? a)
        {
            if (a == null) throw new ArgumentNullException("a");
        }

        private void GuardedWithWrongName(string a)
        {
            if (a == null) throw new ArgumentNullException("b");
        }

        private void FirstArgumentMayBeNull(string nullIsValid, string nullIsNotValid)
        {
            if (nullIsNotValid == null) throw new ArgumentNullException("nullIsNotValid");
        }
    }
}
