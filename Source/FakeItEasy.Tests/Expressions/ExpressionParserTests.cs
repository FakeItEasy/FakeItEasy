namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Expressions;
    using NUnit.Framework;
    using TestHelpers;

    [TestFixture]
    public class ExpressionParserTests
    {
        private ExpressionParser parser;

        [SetUp]
        public void Setup()
        {
            this.parser = new ExpressionParser(new CallExpressionParser());
        }

        [Test]
        public void GetFakeObjectCallIsMadeOn_should_be_null_guarded()
        {
            // Arrange
            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                this.parser.GetFakeManagerCallIsMadeOn(CreateDummyExpression()));
        }

        [Test]
        public void GetFakeObjectCallIsMadeOn_should_fail_when_call_is_not_on_an_instance()
        {
            // Arrange
            var callSpecification = CreateCall(() => DateTime.Now);

            // Act

            // Assert
            var thrown = Assert.Throws<ArgumentException>(() =>
                this.parser.GetFakeManagerCallIsMadeOn(callSpecification));

            Assert.That(thrown.Message, Is.EqualTo("The specified call is not made on a fake object."));
        }
  
        private static Expression<Func<T>> CreateCall<T>(Expression<Func<T>> expression)
        {
            return expression;
        }

        private static LambdaExpression CreateDummyExpression()
        {
            return ExpressionHelper.CreateExpression<IFoo>(x => x.Bar());
        }
    }
}