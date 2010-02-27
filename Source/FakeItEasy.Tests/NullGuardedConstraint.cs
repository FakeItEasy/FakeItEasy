using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Configuration;
using FakeItEasy.Api;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FakeItEasy.Tests
{
    /// <summary>
    /// Validates that calls are properly null guarded.
    /// </summary>
    public class NullGuardedConstraint
        : Constraint
    {
        private ConstraintState state;

        /// <summary>
        /// Checks that non all of the parameters of the specified expression are
        /// properly null guarded.
        /// </summary>
        /// <param name="actual">A <see cref="System.Linq.Expression{System.Action}" /> specifying the call to be checked.</param>
        /// <returns>True if it's properly guarded, false if not.</returns>
        /// <exception cref="ArgumentNullException">The specified value is null.</exception>
        /// <exception cref="ArgumentException">The specified value is not a <see cref="System.Linq.Expression{System.Action}" />.</exception>
        public override bool Matches(object actual)
        {
            if (actual == null) throw new ArgumentNullException("actual");

            var expression = actual as Expression<Action>;

            if (expression == null) throw new ArgumentException("The value passed to a NullGuardedConstraint must be of the type System.Linq.Expression<System.Action>.", "actual");

            return this.Matches(expression);
        }

        private bool Matches(Expression<Action> expression)
        {
            this.state = CreateCall(expression);
            return this.state.Matches();
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate(CommonExtensions.FormatInvariant("Calls to {0} should be null guarded.", this.state.ToString()));
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteLine("When called with the following arguments the method did not throw the apporpriate exception:");

            foreach (var failingCall in this.state.GetFailingCallsDescriptions())
            {
                writer.Write("            ");
                writer.WriteLine(failingCall);   
            }
        }

        private static ConstraintState CreateCall(Expression<Action> methodCall)
        {
            var methodExpression = methodCall.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                return new MethodCallConstraintState(methodExpression);
            }

            return new ConstructorCallConstraintState((NewExpression)methodCall.Body);
        }

        private static object GetValueProducedByExpression(Expression expression)
        {
            if (expression == null) return null;

            var lambda = Expression.Lambda(expression);
            return lambda.Compile().DynamicInvoke();
        }


        /// <summary>
        /// Asserts that the specified call is properly null guarded.
        /// </summary>
        /// <param name="call">The call to validate.</param>
        /// <exception cref="ArgumentNullException">The call was null.</exception>
        public static void Assert(Expression<Action> call)
        {
            if (call == null) throw new ArgumentNullException("call");

            NUnit.Framework.Assert.That(call, new NullGuardedConstraint());
        }

        private abstract class ConstraintState
        {
            protected readonly IEnumerable<ArgumentInfo> ValidArguments;
            private MethodBase method;
            IEnumerable<CallThatShouldThrow> unguardedCalls;

            protected ConstraintState(IEnumerable<ArgumentInfo> arguments, MethodBase method)
            {
                this.ValidArguments = arguments;
                this.method = method;
            }

            private IEnumerable<object> ArgumentValues
            {
                get
                {
                    return this.ValidArguments.Select(x => x.Value);
                }
            }

            private IEnumerable<CallThatShouldThrow> GetArgumentsForCallsThatAreNotProperlyNullGuarded()
            {
                var result = new List<CallThatShouldThrow>();

                foreach (var callThatShouldThrow in this.GetArgumentPermutationsThatShouldThrow())
                {
                    try
                    {
                        this.PerformCall(callThatShouldThrow.Arguments);
                        result.Add(callThatShouldThrow);
                    }
                    catch (TargetInvocationException ex)
                    {
                        callThatShouldThrow.Thrown = ex.InnerException;

                        var nullException = ex.InnerException as ArgumentNullException;
                        if (nullException == null || !callThatShouldThrow.ArgumentName.Equals(nullException.ParamName))
                        {
                            result.Add(callThatShouldThrow);
                        }
                    }
                }

                return result;
            }

            private IEnumerable<CallThatShouldThrow> GetArgumentPermutationsThatShouldThrow()
            {
                var result = new List<CallThatShouldThrow>();
                int index = 0;

                foreach (var argument in this.ValidArguments)
                {
                    if (argument.Value != null && IsNullableType(argument.ArgumentType))
                    {
                        var permutation = new CallThatShouldThrow();
                        permutation.ArgumentName = argument.Name;
                        permutation.Arguments = this.ArgumentValues.Take(index).Concat(new object[] { null }).Concat(this.ArgumentValues.Skip(index + 1)).ToArray();
                        result.Add(permutation);
                    }

                    index++;
                }

                return result;
            }

            private static bool IsNullableType(Type type)
            {
                return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
            }

            public bool Matches()
            {
                this.unguardedCalls = this.GetArgumentsForCallsThatAreNotProperlyNullGuarded();

                return unguardedCalls.Count() == 0;
            }

            protected static IEnumerable<ArgumentInfo> GetArgumentInfos(IEnumerable<Expression> callArgumentsValues, ParameterInfo[] callArguments)
            {
                var result = new List<ArgumentInfo>();
                int index = 0;
                foreach (var argument in callArgumentsValues)
                {
                    result.Add(new ArgumentInfo()
                    {
                        ArgumentType = callArguments[index].ParameterType,
                        Name = callArguments[index].Name,
                        Value = GetValueProducedByExpression(argument)
                    });

                    index++;
                }

                return result;
            }

            protected abstract string CallDescription { get; }
            protected abstract void PerformCall(object[] arguments);

            private class CallThatShouldThrow
            {
                public string ArgumentName;
                public object[] Arguments;
                public Exception Thrown;
            }

            protected struct ArgumentInfo
            {
                public string Name;
                public Type ArgumentType;
                public object Value;
            }

            public override string ToString()
            {
                var result = new StringBuilder();
                result.Append(this.CallDescription);
                result.Append("(");

                AppendArgumentList(result, this.ValidArguments);
                result.Append(")");

                return result.ToString();
            }

            private static void AppendArgumentList(StringBuilder builder, IEnumerable<ArgumentInfo> arguments)
            {
                int lengthWhenStarting = builder.Length;
                foreach (var argument in arguments)
                {
                    if (builder.Length > lengthWhenStarting)
                    {
                        builder.Append(", ");
                    }
                    
                    AppendArgument(builder, argument);
                }
            }

            private static void AppendArgumentList(StringBuilder builder, IEnumerable<object> arguments)
            {
                int lengthWhenStarting = builder.Length;
                foreach (var argument in arguments)
                {
                    if (builder.Length > lengthWhenStarting)
                    {
                        builder.Append(", ");
                    }

                    AppendArgument(builder, argument);
                }
            }

            private static void AppendArgument(StringBuilder builder, ArgumentInfo argument)
            {
                builder.Append("[");
                builder.Append(argument.ArgumentType.Name);
                builder.Append("]");
                builder.Append(" ");
                builder.Append(argument.Name);
            }

            private static void AppendArgument(StringBuilder builder, object argument)
            {
                if (argument == null)
                {
                    builder.Append("<NULL>");
                    return;
                }

                if (argument is string)
                {
                    builder.AppendFormat("\"{0}\"", argument);
                }
                else
                {
                    builder.Append(argument);
                }
            }

            public IEnumerable<string> GetFailingCallsDescriptions()
            {
                var descriptions = new List<string>();
                
                foreach (var call in this.unguardedCalls)
                {
                    var description = GetFailingCallDescription(call);
                    descriptions.Add(description);
                }

                return descriptions;
            }

            private static string GetFailingCallDescription(CallThatShouldThrow call)
            {
                var result = new StringBuilder();
                
                result.Append("(");
                AppendArgumentList(result, call.Arguments);
                result.Append(") ");

                AppendFailReason(result, call);

                return result.ToString();
            }

            private static void AppendFailReason(StringBuilder description, CallThatShouldThrow call)
            {
                if (call.Thrown == null)
                {
                    description.Append("did not throw any exception.");
                }
                else
                {
                    var argumentNullException = call.Thrown as ArgumentNullException;
                    if (argumentNullException != null)
                    {
                        description.Append(CommonExtensions.FormatInvariant("threw ArgumentNullException with wrong argument name, it should be \"{0}\".", call.ArgumentName));
                    }
                    else
                    {
                        description.Append(CommonExtensions.FormatInvariant("threw unexpected {0}.", call.Thrown.GetType().FullName));
                    }
                }
            }
        }

        private class MethodCallConstraintState
            : ConstraintState
        {
            private object target;
            private MethodInfo method;

            public MethodCallConstraintState(MethodCallExpression expression)
                : base(GetExpressionArguments(expression), expression.Method)
            {
                this.method = expression.Method;
                this.target = GetValueProducedByExpression(expression.Object);
            }

            private static IEnumerable<ArgumentInfo> GetExpressionArguments(MethodCallExpression expression)
            {
                return GetArgumentInfos(expression.Arguments, expression.Method.GetParameters());
            }

            protected override void PerformCall(object[] arguments)
            {
                this.method.Invoke(this.target, arguments);
            }

            protected override string CallDescription
            {
                get { return this.method.ReflectedType.Name + "." + this.method.Name; }
            }
        }

        private class ConstructorCallConstraintState
            : ConstraintState
        {
            private ConstructorInfo constructorInfo;

            public ConstructorCallConstraintState(NewExpression expression)
                : base(GetArgumentInfos(expression), expression.Constructor)
            {
                this.constructorInfo = expression.Constructor;
            }

            private static IEnumerable<ArgumentInfo> GetArgumentInfos(NewExpression expression)
            {
                return GetArgumentInfos(expression.Arguments, expression.Constructor.GetParameters());
            }

            protected override void PerformCall(object[] arguments)
            {
                this.constructorInfo.Invoke(arguments);
            }

            protected override string CallDescription
            {
                get { return this.constructorInfo.ReflectedType.FullName + ".ctor"; }
            }
        }
    }
}
