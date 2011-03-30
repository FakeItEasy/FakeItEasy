using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FakeItEasy.Core;

namespace FakeItEasy.Tests.Expressions
{
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;
    using FakeItEasy.Expressions.ArgumentConstraints;

    [TestFixture]
    public class ArgumentConstraintFactoryTests
    {
        private ArgumentConstraintFactory CreateFactory()
        {
            return new ArgumentConstraintFactory();
        }

        [Test]
        public void GetArgumentConstraint_should_get_constraint_produced_by_method()
        {
            // Arrange
            var constraint = A.Fake<ArgumentConstraint<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(GetConstraint(constraint)), 0);

            // Act
            var factory = this.CreateFactory();
            var producedConstraint = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedConstraint, Is.SameAs(constraint));
        }

        [Test]
        public void GetArgumentConstraint_should_get_constraint_passed_in_directly()
        {
            // Arrange
            var constraint = A.Fake<ArgumentConstraint<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(constraint), 0);
            
            // Act
            var factory = this.CreateFactory();
            var producedConstraint = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedConstraint, Is.SameAs(constraint));
        }

        [Test]
        public void GetArgumentConstraint_should_get_constraint_when_Argument_property_has_been_called_on_it()
        {
            // Arrange
            var constraint = A.Fake<ArgumentConstraint<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(constraint.Argument), 0);

            // Act
            var factory = this.CreateFactory();
            var producedConstraint = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedConstraint, Is.SameAs(constraint));
        }

        [Test]
        public void GetArgumentConstraint_should_get_constraint_when_parameter_is_of_type_object()
        {
            // Arrange
            var constraint = A.Fake<ArgumentConstraint<object>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Bar(GetConstraint(constraint)), 0);

            // Act
            var factory = this.CreateFactory();
            var producedConstraint = factory.GetArgumentConstraint(argument);

            // Assert
            Assert.That(producedConstraint, Is.SameAs(constraint));
        }

        [Test]
        public void Should_get_argument_constraint_that_is_trapped()
        {

        }

        private static ArgumentConstraint<T> GetConstraint<T>(ArgumentConstraint<T> constraint)
        {
            return constraint;
        }

        public interface ITestInterface
        {
            void Foo(int number);
            void Bar(object value);
        }
    }

    [TestFixture]
    public class ExpressionArgumentConstraintFactoryTests
    {
        private IArgumentConstraintTrapper trapper;
        private ExpressionArgumentConstraintFactory factory;

        [SetUp]
        public void SetUp()
        {
            this.trapper = A.Fake<IArgumentConstraintTrapper>();

            this.factory = new ExpressionArgumentConstraintFactory(this.trapper);
        }

        [Test]
        public void Should_return_constraint_from_trapper_when_available()
        {
            // Arrange
            var constraint = A.Dummy<IArgumentConstraint>();
            Any.CallTo(this.trapper).WithReturnType<IEnumerable<IArgumentConstraint>>().Returns(new[] {constraint});

            // Act
            var result = this.factory.GetArgumentConstraint(A.Dummy<Expression>());

            // Assert
            Assert.That(result, Is.SameAs(constraint));
        }

        [Test]
        public void Should_return_equality_constraint_when_trapper_doesnt_produce_any_constraint()
        {
            // Arrange
            Any.CallTo(this.trapper).WithReturnType<IEnumerable<IArgumentConstraint>>()
                .Invokes(x => x.GetArgument<Action>(0).Invoke())
                .Returns(Enumerable.Empty<IArgumentConstraint>());

            // Act
            var result = this.factory.GetArgumentConstraint(Expression.Constant("foo"));

            // Assert
            Assert.That(result, Is.InstanceOf<EqualityArgumentConstraint>().And.Property("ExpectedValue").EqualTo("foo"));
        }

        [Test]
        public void Should_pass_action_that_invokes_expression_to_trapper()
        {
            // Arrange
            bool wasInvoked = false;
            Action invokation = () => wasInvoked = true;
            var expression = Expression.Call(Expression.Constant(invokation), typeof (Action).GetMethod("Invoke"));
            
            // Act
            this.factory.GetArgumentConstraint(expression);

            // Assert
            var actionToTrapper = Fake.GetCalls(this.trapper).Single().GetArgument<Action>(0);
            wasInvoked = false;
            actionToTrapper.Invoke();

            Assert.That(wasInvoked, Is.True);
        }

        [Test]
        public void Should_not_invoke_expression_more_than_once()
        {
            // Arrange
            int invokedNumberOfTimes = 0;
            Action invokation = () => invokedNumberOfTimes++;
            var expression = Expression.Call(Expression.Constant(invokation), typeof(Action).GetMethod("Invoke"));
            
            A.CallTo(() => this.trapper.TrapConstraints(null)).WithAnyArguments()
                .Invokes(x => x.GetArgument<Action>(0).Invoke())
                .Returns(Enumerable.Empty<IArgumentConstraint>());
            
            // Act
            this.factory.GetArgumentConstraint(expression);

            // Assert
            Assert.That(invokedNumberOfTimes, Is.EqualTo(1));
        }
    }
}
