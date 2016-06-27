namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ParsedCallExpression
    {
        private readonly Lazy<object> callTarget;

        public ParsedCallExpression(
            MethodInfo calledMethod,
            Expression callTargetExpression,
            IEnumerable<ParsedArgumentExpression> argumentsExpressions)
        {
            this.CalledMethod = calledMethod;
            this.ArgumentsExpressions = argumentsExpressions;
            this.callTarget = new Lazy<object>(() => callTargetExpression?.Evaluate());
        }

        public ParsedCallExpression(
            MethodInfo calledMethod,
            object callTarget,
            IEnumerable<ParsedArgumentExpression> argumentsExpressions)
        {
            this.CalledMethod = calledMethod;
            this.ArgumentsExpressions = argumentsExpressions;
            this.callTarget = new Lazy<object>(() => callTarget);
        }

        public MethodInfo CalledMethod { get; }

        public IEnumerable<ParsedArgumentExpression> ArgumentsExpressions { get; }

        public object CallTarget => this.callTarget.Value;
    }
}
