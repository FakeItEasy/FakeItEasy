namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions.Execution;

    public static class NullGuardedAssertion
    {
        public static NullGuardedConstraint Should(this Expression<Action> call)
        {
            return new NullGuardedConstraint(call);
        }

        /// <summary>
        /// Validates that calls are properly null guarded.
        /// </summary>
        public class NullGuardedConstraint
        {
            private readonly Expression<Action> call;
            private ConstraintState state;

            internal NullGuardedConstraint(Expression<Action> call)
            {
                Guard.AgainstNull(call, nameof(call));
                this.call = call;
            }

            public void BeNullGuarded()
            {
                var matches = this.Matches(this.call);
                if (matches)
                {
                    return;
                }

                var builder = ServiceLocator.Current.Resolve<StringBuilderOutputWriter>();
                this.WriteDescriptionTo(builder);
                builder.WriteLine();
                this.WriteActualValueTo(builder);
                var reason = builder.Builder.ToString();
                Execute.Assertion.FailWith(reason);
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
                if (expression == null)
                {
                    return null;
                }

                var lambda = Expression.Lambda(expression);
                return lambda.Compile().DynamicInvoke();
            }

            private void WriteDescriptionTo(StringBuilderOutputWriter builder)
            {
                builder.Write("Expected calls to ");
                this.state.WriteTo(builder);
                builder.Write(" to be null guarded.");
            }

            private void WriteActualValueTo(StringBuilderOutputWriter builder)
            {
                builder.Write(
                    "When called with the following arguments the method did not throw the appropriate exception:");
                builder.WriteLine();
                this.state.WriteFailingCallsDescriptions(builder);
            }

            private bool Matches(Expression<Action> expression)
            {
                this.state = CreateCall(expression);
                return this.state.Matches();
            }

            private abstract class ConstraintState
            {
                protected readonly IEnumerable<ArgumentInfo> ValidArguments;
                private IEnumerable<CallThatShouldThrow> unguardedCalls;

                protected ConstraintState(IEnumerable<ArgumentInfo> arguments)
                {
                    this.ValidArguments = arguments;
                }

                protected abstract string CallDescription { get; }

                private IEnumerable<object> ArgumentValues
                {
                    get { return this.ValidArguments.Select(x => x.Value); }
                }

                public bool Matches()
                {
                    this.unguardedCalls = this.GetArgumentsForCallsThatAreNotProperlyNullGuarded();

                    return !this.unguardedCalls.Any();
                }

                public void WriteTo(StringBuilderOutputWriter builder)
                {
                    builder.Write(this.CallDescription);
                    builder.Write("(");

                    WriteArgumentList(builder, this.ValidArguments);
                    builder.Write(")");
                }

                public void WriteFailingCallsDescriptions(StringBuilderOutputWriter builder)
                {
                    using (builder.Indent())
                    {
                        foreach (var call in this.unguardedCalls)
                        {
                            call.WriteFailingCallDescription(builder);
                            builder.WriteLine();
                        }
                    }
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

                protected abstract void PerformCall(object[] arguments);

                private static bool IsNullableType(Type type)
                {
                    return !type.GetTypeInformation().IsValueType ||
                           (type.GetTypeInformation().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
                }

                private static void WriteArgumentList(StringBuilderOutputWriter builder, IEnumerable<ArgumentInfo> arguments)
                {
                    int lengthWhenStarting = builder.Builder.Length;
                    foreach (var argument in arguments)
                    {
                        if (builder.Builder.Length > lengthWhenStarting)
                        {
                            builder.Write(", ");
                        }

                        WriteArgument(builder, argument);
                    }
                }

                private static void WriteArgument(StringBuilderOutputWriter builder, ArgumentInfo argument)
                {
                    builder.Write("[");
                    builder.Write(argument.ArgumentType.Name);
                    builder.Write("]");
                    builder.Write(" ");
                    builder.Write(argument.Name);
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
                            callThatShouldThrow.SetThrownException(ex.InnerException);

                            var nullException = ex.InnerException as ArgumentNullException;
                            if (nullException == null ||
                                callThatShouldThrow.ArgumentName != nullException.ParamName)
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
                            var permutation = new CallThatShouldThrow
                            {
                                ArgumentName = argument.Name,
                                Arguments = this.ArgumentValues.Take(index)
                                    .Concat(default(object))
                                    .Concat(this.ArgumentValues.Skip(index + 1))
                                    .ToArray()
                            };
                            result.Add(permutation);
                        }

                        index++;
                    }

                    return result;
                }

                protected struct ArgumentInfo
                {
                    public string Name { get; set; }

                    public Type ArgumentType { get; set; }

                    public object Value { get; set; }
                }

                private class CallThatShouldThrow
                {
                    private Exception thrown;

                    public string ArgumentName { get; set; }

                    public object[] Arguments { get; set; }

                    public void SetThrownException(Exception value)
                    {
                        this.thrown = value;
                    }

                    public void WriteFailingCallDescription(StringBuilderOutputWriter builder)
                    {
                        builder.Write("(");
                        this.WriteArgumentList(builder);
                        builder.Write(") ");
                        this.WriteFailReason(builder);
                    }

                    private void WriteArgumentList(StringBuilderOutputWriter builder)
                    {
                        int lengthWhenStarting = builder.Builder.Length;
                        foreach (var argument in this.Arguments)
                        {
                            if (builder.Builder.Length > lengthWhenStarting)
                            {
                                builder.Write(", ");
                            }

                            builder.WriteArgumentValue(argument);
                        }
                    }

                    private void WriteFailReason(StringBuilderOutputWriter description)
                    {
                        if (this.thrown == null)
                        {
                            description.Write("did not throw any exception.");
                        }
                        else
                        {
                            var argumentNullException = this.thrown as ArgumentNullException;
                            if (argumentNullException != null)
                            {
                                description.Write(
                                    $"threw ArgumentNullException with wrong argument name, it should be {this.ArgumentName}.");
                            }
                            else
                            {
                                description.Write($"threw unexpected {this.thrown.GetType()}.");
                            }
                        }
                    }
                }
            }

            private class MethodCallConstraintState
                : ConstraintState
            {
                private readonly object target;
                private readonly MethodInfo method;

                public MethodCallConstraintState(MethodCallExpression expression)
                    : base(GetExpressionArguments(expression))
                {
                    this.method = expression.Method;
                    this.target = NullGuardedConstraint.GetValueProducedByExpression(expression.Object);
                }

#if FEATURE_NETCORE_REFLECTION
                protected override string CallDescription => this.method.DeclaringType.Name + "." + this.method.Name;
#else
                protected override string CallDescription => this.method.ReflectedType.Name + "." + this.method.Name;
#endif

                protected override void PerformCall(object[] arguments)
                {
                    this.method.Invoke(this.target, arguments);
                }

                private static IEnumerable<ArgumentInfo> GetExpressionArguments(MethodCallExpression expression)
                {
                    return ConstraintState.GetArgumentInfos(expression.Arguments, expression.Method.GetParameters());
                }
            }

            private class ConstructorCallConstraintState
                : ConstraintState
            {
                private readonly ConstructorInfo constructorInfo;

                public ConstructorCallConstraintState(NewExpression expression)
                    : base(GetArgumentInfos(expression))
                {
                    this.constructorInfo = expression.Constructor;
                }

#if FEATURE_NETCORE_REFLECTION
                protected override string CallDescription => this.constructorInfo.DeclaringType.ToString() + ".ctor";
#else
                protected override string CallDescription => this.constructorInfo.ReflectedType.ToString() + ".ctor";
#endif

                protected override void PerformCall(object[] arguments)
                {
                    this.constructorInfo.Invoke(arguments);
                }

                private static IEnumerable<ArgumentInfo> GetArgumentInfos(NewExpression expression)
                {
                    return GetArgumentInfos(expression.Arguments, expression.Constructor.GetParameters());
                }
            }
        }
    }
}
