﻿namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ParsedArgumentExpression
    {
        public ParsedArgumentExpression(Expression expression, ParameterInfo argumentInformation)
        {
            this.Expression = expression;
            this.ArgumentInformation = argumentInformation;
        }

        public Expression Expression { get; private set; }

        public object Value
        {
            get { return Helpers.GetValueProducedByExpression(this.Expression); }
        }

        public ParameterInfo ArgumentInformation { get; private set; }
    }
}