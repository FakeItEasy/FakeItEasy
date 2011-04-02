namespace FakeItEasy.Expressions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ParsedCallExpression
    {
        public ParsedCallExpression(MethodInfo calledMethod, Expression callTargetExpression, IEnumerable<Expression> argumentsExpressions)
        {
            this.CalledMethod = calledMethod;
            this.CallTargetExpression = callTargetExpression;
            this.ArgumentsExpressions = argumentsExpressions;
        }

        public MethodInfo CalledMethod { get; private set; }

        public Expression CallTargetExpression { get; private set; }

        public IEnumerable<Expression> ArgumentsExpressions { get; private set; }

        public object CallTarget
        {
            get
            {
                return this.CallTargetExpression != null
                           ? Helpers.GetValueProducedByExpression(this.CallTargetExpression)
                           : null;
            }
        }

        public IEnumerable<object> Arguments
        {
            get { return this.ArgumentsExpressions.Select(x => Helpers.GetValueProducedByExpression(x)); }
        }
    }
}