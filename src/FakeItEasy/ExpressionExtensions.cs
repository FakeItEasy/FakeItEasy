namespace FakeItEasy
{
    using System.Linq;
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
            return EvaluateOptimized(expression);
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
        // - trivial method calls with no arguments (A.ToString(), B.GetType(), Factory.Create())
        private static object EvaluateOptimized(this Expression expression)
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
                        return fieldInfo.GetValue(EvaluateOptimized(memberExpression.Expression));
                    }

                    var propertyInfo = memberExpression.Member as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        // index = null: this is always fine since it's a MemberAccess expression, not an IndexExpression
                        return propertyInfo.GetValue(EvaluateOptimized(memberExpression.Expression), null);
                    }

                    break;

                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;

                    // for now, handling only very trivial call expressions with no arguments, like 'GetBlarg()'
                    // because anything else might get complicated quickly (method overloads etc.)
                    if (!callExpression.Arguments.Any())
                    {
                        var targetObject = EvaluateOptimized(callExpression.Object);
                        return callExpression.Method.Invoke(targetObject, null);
                    }

                    break;

                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;

                    // for now, handling only 'boxing/casting to object' expressions like '3' used as an argument to a function which takes an object[] array parameter.
                    if (unaryExpression.Type == typeof(object))
                    {
                        // in principle, we would first evaluate it without the boxing, and then box/cast to object...
                        // ...but EvaluateOptimized already boxes before returning, so no explicit cast needed.
                        object obj = EvaluateOptimized(unaryExpression.Operand); // declaring an explicitly typed variable to ensure we're really boxing
                        return obj;
                    }

                    break;
            }

            // when we didn't know how to evaluate the tree, fall back to compiling into a delegate and invoking the expression
            var lambda = Expression.Lambda(expression).Compile();
            return lambda.DynamicInvoke();
        }
    }
}
