namespace FakeItEasy.Expressions
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ParsedCallExpression
    {
        public ParsedCallExpression(MethodInfo calledMethod, Expression callTargetExpression, IEnumerable<ParsedArgumentExpression> argumentsExpressions)
        {
            this.CalledMethod = calledMethod;
            this.CallTargetExpression = callTargetExpression;
            this.ArgumentsExpressions = argumentsExpressions;
        }

        public MethodInfo CalledMethod { get; private set; }

        public Expression CallTargetExpression { get; private set; }

        public IEnumerable<ParsedArgumentExpression> ArgumentsExpressions { get; private set; }

        public object CallTarget
        {
            get { return this.CallTargetExpression != null ? this.CallTargetExpression.Evaluate() : null; }
        }

        // TODO (adamralph): remove suppression when SL is dropped or add an SL test which uses this
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by non-SL test projects.")]
        public IEnumerable<object> Arguments
        {
            get { return this.ArgumentsExpressions.Select(x => x.Value); }
        }
    }
}