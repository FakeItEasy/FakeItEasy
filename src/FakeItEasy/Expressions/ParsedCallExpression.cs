namespace FakeItEasy.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public MethodInfo CalledMethod { get; private set; }

        public IEnumerable<ParsedArgumentExpression> ArgumentsExpressions { get; private set; }

        public object CallTarget => this.callTarget.Value;

        public IEnumerable<object> Arguments
        {
            get { return this.ArgumentsExpressions.Select(x => x.Value); }
        }
    }
}
