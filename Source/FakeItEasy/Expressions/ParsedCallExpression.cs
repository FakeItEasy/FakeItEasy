namespace FakeItEasy.Expressions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ParsedCallExpression
    {
        public ParsedCallExpression(
            MethodInfo calledMethod,
            Expression callTargetExpression,
            IEnumerable<ParsedArgumentExpression> argumentsExpressions)
        {
            this.CalledMethod = calledMethod;
            this.CallTargetExpression = callTargetExpression;
            this.ArgumentsExpressions = argumentsExpressions;
        }

        public MethodInfo CalledMethod { get; private set; }

        public IEnumerable<ParsedArgumentExpression> ArgumentsExpressions { get; private set; }

        public object CallTarget
        {
            get { return this.CallTargetExpression != null ? this.CallTargetExpression.Evaluate() : null; }
        }

        public IEnumerable<object> Arguments
        {
            get { return this.ArgumentsExpressions.Select(x => x.Value); }
        }

        private Expression CallTargetExpression { get; set; }
    }
}