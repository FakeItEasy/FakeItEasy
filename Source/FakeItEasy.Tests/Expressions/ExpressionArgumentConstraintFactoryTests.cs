using FakeItEasy.Expressions.ArgumentConstraints;

namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using NUnit.Framework;

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
            Any.CallTo(this.trapper).WithReturnType<IEnumerable<IArgumentConstraint>>().Returns(new[] { constraint });

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