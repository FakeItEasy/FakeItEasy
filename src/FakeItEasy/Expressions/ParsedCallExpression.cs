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
            this.callTarget = new Lazy<object>(() => FastEvaluate(callTargetExpression));
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

        // Expression evaluation optimized for a very common case,
        // locating the call target described by a local lambda closure
        // such as
        //   A.CallTo(() => fakeObj.doSomething(args))
        //
        // In this example, fakeObj is a field on the anonymous
        // type that the compiler creates to represent the closure,
        // so the expression will be a MemberExpression where the
        // Member is a FieldInfo.
        // That FieldInfo can be used to find the value of fakeObj
        // from the anonymous object, which is represented in the
        // expression tree by the ConstantExpression.
        private static object FastEvaluate(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).Value;

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var fieldInfo = memberExpression.Member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        return fieldInfo.GetValue(FastEvaluate(memberExpression.Expression));
                    }

                    break;
            }

            return expression.Evaluate();
        }
    }
}
