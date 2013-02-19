﻿namespace FakeItEasy.Tests.Builders
{
    using System;
    using System.Linq.Expressions;

    internal class BuilderForExpression : TestDataBuilder<Expression, BuilderForExpression>
    {
        Expression builtExpression;

        public BuilderForExpression()
        {
            this.builtExpression = Expression.Constant(null);
        }

        public BuilderForExpression Constant(object value)
        {
            return this.Do(x => x.builtExpression = Expression.Constant(value));
        }

        public BuilderForExpression Call(Expression<Action> callSpecification)
        {
            return this.Do(x => x.builtExpression = callSpecification.Body);
        }
      
        public static Expression GetBody<T>(Expression<Func<T>> expression)
        {
            return expression.Body;
        }

        protected override Expression Build()
        {
            return this.builtExpression;
        }
    }
}