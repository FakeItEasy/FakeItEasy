namespace FakeItEasy.Tests.Expressions
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.Builders;
    using FluentAssertions;
    using Xunit;

    public class ExpressionArgumentConstraintFactoryTests
    {
        [Fact]
        public void Should_not_invoke_expression_more_than_once()
        {
            // Arrange
            ExpressionArgumentConstraintFactory factory = new ExpressionArgumentConstraintFactory(new ArgumentConstraintTrap());
            int invokedNumberOfTimes = 0;
            Func<object> invocation = () =>
            {
                invokedNumberOfTimes++;
                return null;
            };
            var expression = BuilderForExpression.GetBody(() => invocation());

            // Act
            factory.GetArgumentConstraint(BuilderForParsedArgumentExpression.Build(x => x.WithExpression(expression)));

            // Assert
            invokedNumberOfTimes.Should().Be(1);
        }
    }
}
