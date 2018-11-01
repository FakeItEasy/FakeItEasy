namespace FakeItEasy
{
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="Expression"/>.
    /// </summary>
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// Evaluates an expression, potentially by compiling it into a delegate and invoking the delegate, but faster if possible.
        /// </summary>
        /// <notes>
        /// This method evaluates an expression, but tries to do it in a light-weight way that doesn't compile it into a delegate.
        /// It is often used to solve 'what object/value does the user-supplied Expression refer to?'
        /// </notes>
        /// <param name="expression">The expression to be evaluated.</param>
        /// <returns>The value returned from the delegate compiled from the expression.</returns>
        public static object Evaluate(this Expression expression)
        {
            if (TryFastEvaluate(expression, out object result))
            {
                return result;
            }

            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        // About the optimizations:
        // Key observation is that walking the expression tree and evaluating directly can be much faster for simple expressions like:
        // - constant expressions (null, 1, "easy", etc.)
        // - local variables referenced from lambda expressions, such as 'fake' in the A.CallTo line:
        //     {
        //         var fake = A.Fake<Something>();
        //         A.CallTo(() => fake.DoStuff());
        //     }
        // - expressions that are simple member accesses (field gets, property gets) on an object or class, such as
        //     String.Empty
        //     A<SomethingA>.Ignored
        //     A<SomethingA>._
        //     myObj.someProperty
        // - method calls
        private static bool TryFastEvaluate(Expression expression, out object result)
        {
            result = null;

            if (expression == null)
            {
                return true;
            }

            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    result = ((ConstantExpression)expression).Value;
                    return true;

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;

                    var fieldInfo = memberExpression.Member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        if (TryFastEvaluate(memberExpression.Expression, out object memberResult))
                        {
                            result = fieldInfo.GetValue(memberResult);
                            return true;
                        }

                        return false;
                    }

                    var propertyInfo = memberExpression.Member as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        // index = null: this is always fine since it's a MemberAccess expression, not an IndexExpression
                        if (TryFastEvaluate(memberExpression.Expression, out object memberResult))
                        {
                            result = propertyInfo.GetValue(memberResult, null);
                            return true;
                        }

                        return false;
                    }

                    break;

                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;

                    if (!TryFastEvaluate(callExpression.Object, out object target))
                    {
                        return false;
                    }

                    object[] argumentValues = new object[callExpression.Arguments.Count];
                    for (int i = 0; i < callExpression.Arguments.Count; i++)
                    {
                        if (!TryFastEvaluate(callExpression.Arguments[i], out argumentValues[i]))
                        {
                            return false;
                        }
                    }

                    result = callExpression.Method.Invoke(target, argumentValues);
                    return true;

                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;

                    // for now, handling only 'boxing/casting to object' expressions like '3' used as an argument to a function which takes an object[] array parameter.
                    if (unaryExpression.Type == typeof(object))
                    {
                        // in principle, we would first evaluate it without the boxing, and then box/cast to object...
                        // ...but TryFastEvaluate already boxes before returning, so no explicit cast needed.
                        if (TryFastEvaluate(unaryExpression.Operand, out object operand))
                        {
                            result = operand;
                            return true;
                        }
                    }

                    break;
            }

            return false;
        }
    }
}
