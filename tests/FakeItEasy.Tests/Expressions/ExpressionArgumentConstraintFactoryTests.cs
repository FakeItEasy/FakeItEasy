namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FakeItEasy.Expressions.ArgumentConstraints;
    using FakeItEasy.Tests.Builders;
    using FluentAssertions;
    using Xunit;

    public class ExpressionArgumentConstraintFactoryTests
    {
        private readonly IArgumentConstraintTrapper trapper;
        private readonly ExpressionArgumentConstraintFactory factory;

        public ExpressionArgumentConstraintFactoryTests()
        {
            this.trapper = A.Fake<IArgumentConstraintTrapper>();

            Fake.GetFakeManager(this.trapper).AddInterceptionListener(new InvokeTrapConstraintsAction());

            this.factory = new ExpressionArgumentConstraintFactory(this.trapper);
        }

        [Fact]
        public void Should_return_constraint_from_trapper_when_available()
        {
            // Arrange
            var constraint = A.Dummy<IArgumentConstraint>();

            A.CallTo(this.trapper).WithReturnType<IEnumerable<IArgumentConstraint>>().Returns(new[] { constraint });

            // Act
            var result = this.factory.GetArgumentConstraint(BuilderForParsedArgumentExpression.BuildWithDefaults());

            // Assert
            result.Should().BeSameAs(constraint);
        }

        [Fact]
        public void Should_return_equality_constraint_when_trapper_does_not_produce_any_constraint()
        {
            // Arrange
            A.CallTo(() => this.trapper.TrapConstraints(A<Action>._))
                .Returns(Enumerable.Empty<IArgumentConstraint>());

            // Act
            var result = this.factory.GetArgumentConstraint(BuilderForParsedArgumentExpression.Build(x => x.WithConstantExpression("foo")));

            // Assert
            result.Should().BeOfType<EqualityArgumentConstraint>().Which.ExpectedValue.Should().Be("foo");
        }

        [Fact]
        public void Should_pass_action_that_invokes_expression_to_trapper()
        {
            // Arrange
            bool wasInvoked = false;
            var invocation = FuncFromAction(() => wasInvoked = true);

            var expression = BuilderForExpression.GetBody(() => invocation());

            // Act
            this.factory.GetArgumentConstraint(BuilderForParsedArgumentExpression.Build(x => x.WithExpression(expression)));

            // Assert
            var actionToTrapper = Fake.GetCalls(this.trapper).Single().GetArgument<Action>(0);
            wasInvoked = false;
            actionToTrapper.Invoke();

            wasInvoked.Should().BeTrue();
        }

        [Fact]
        public void Should_not_invoke_expression_more_than_once()
        {
            // Arrange
            int invokedNumberOfTimes = 0;
            var invocation = FuncFromAction(() => invokedNumberOfTimes++);

            var expression = BuilderForExpression.GetBody(() => invocation());

            this.StubTrapperToReturnNoConstraints();

            // Act
            this.factory.GetArgumentConstraint(BuilderForParsedArgumentExpression.Build(x => x.WithExpression(expression)));

            // Assert
            invokedNumberOfTimes.Should().Be(1);
        }

        [Fact]
        public void Should_get_aggregate_constraint_when_multiple_items_are_passed_to_parameters_array()
        {
            // Arrange
            var constraintForFirst = A.CollectionOfFake<IArgumentConstraint>(1);
            var noConstraintForSecond = Enumerable.Empty<IArgumentConstraint>();
            var constraintForThird = A.CollectionOfFake<IArgumentConstraint>(1);

            A.CallTo(() => this.trapper.TrapConstraints(A<Action>._))
                .ReturnsNextFromSequence(constraintForFirst, noConstraintForSecond, constraintForThird);

            var expression = this.FromExpression(() => this.MethodWithParamArray(A<string>._, "foo", A<string>._));

            // Act
            var result = this.factory.GetArgumentConstraint(expression);

            // Assert
            result.Should().BeOfType<AggregateArgumentConstraint>();
            var aggregate = (AggregateArgumentConstraint)result;
            aggregate.Constraints.Should().HaveCount(3);
            aggregate.Constraints.ElementAt(0).Should().BeSameAs(constraintForFirst.Single());
            aggregate.Constraints.ElementAt(1).Should().BeOfType<EqualityArgumentConstraint>().Which.ExpectedValue.Should().Be("foo");
            aggregate.Constraints.ElementAt(2).Should().BeSameAs(constraintForThird.Single());
        }

        [Fact]
        public void Should_get_equality_constraint_when_array_is_passed_to_parameters_array()
        {
            // Arrange
            this.StubTrapperToReturnNoConstraints();
            var someStrings = new[] { "foo", "bar" };
            var expression = this.FromExpression(() => this.MethodWithParamArray(someStrings));

            // Act
            var result = this.factory.GetArgumentConstraint(expression);

            // Assert
            result.Should().BeOfType<EqualityArgumentConstraint>();
        }

        [Fact]
        public void Should_get_equality_constraint_when_null_is_passed_to_parameters_array()
        {
            // Arrange
            this.StubTrapperToReturnNoConstraints();
            var expression = this.FromExpression(() => this.MethodWithParamArray(null));

            // Act
            var result = this.factory.GetArgumentConstraint(expression);

            // Assert
            result.Should().BeOfType<EqualityArgumentConstraint>();
        }

        private static Func<object> FuncFromAction(Action action)
        {
            return () =>
            {
                action();
                return null;
            };
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args", Justification = "Required for testing.")]
        private void MethodWithParamArray(params string[] args)
        {
        }

        private ParsedArgumentExpression FromExpression(Expression<Action> fromFirstArgumentInCall)
        {
            return BuilderForParsedArgumentExpression.Build(x => x.FromFirstArgumentInMethodCall(fromFirstArgumentInCall));
        }

        private void StubTrapperToReturnNoConstraints()
        {
            A.CallTo(() => this.trapper.TrapConstraints(A<Action>._))
                .Returns(Enumerable.Empty<IArgumentConstraint>());
        }

        private class InvokeTrapConstraintsAction : IInterceptionListener
        {
            private readonly ArgumentConstraintTrap realTrap = new ArgumentConstraintTrap();

            public void OnBeforeCallIntercepted(IFakeObjectCall call)
            {
            }

            public void OnAfterCallIntercepted(ICompletedFakeObjectCall call, IFakeObjectCallRule ruleThatWasApplied)
            {
                Guard.AgainstNull(call, nameof(call));

                if (call.Method.Name == "TrapConstraints")
                {
                    this.realTrap.TrapConstraints(call.GetArgument<Action>(0));
                }
            }
        }
    }
}
