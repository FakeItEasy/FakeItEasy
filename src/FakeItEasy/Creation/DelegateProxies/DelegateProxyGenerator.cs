namespace FakeItEasy.Creation.DelegateProxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    internal class DelegateProxyGenerator
        : IProxyGenerator
    {
        public virtual ProxyGeneratorResult GenerateProxy(
            Type typeOfProxy,
            IEnumerable<Type> additionalInterfacesToImplement,
            IEnumerable<object> argumentsForConstructor,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(typeOfProxy, nameof(typeOfProxy));

            if (!typeof(Delegate).IsAssignableFrom(typeOfProxy))
            {
                return new ProxyGeneratorResult("The delegate proxy generator can only create proxies for delegate types.");
            }

            var invokeMethod = typeOfProxy.GetMethod("Invoke");
            var eventRaiser = new DelegateCallInterceptedEventRaiser(fakeCallProcessorProvider, invokeMethod, typeOfProxy);

            fakeCallProcessorProvider.EnsureInitialized(eventRaiser.Instance);
            return new ProxyGeneratorResult(eventRaiser.Instance);
        }

        public virtual ProxyGeneratorResult GenerateProxy(
            Type typeOfProxy,
            IEnumerable<Type> additionalInterfacesToImplement,
            IEnumerable<object> argumentsForConstructor,
            IEnumerable<Expression<Func<Attribute>>> attributes,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            return this.GenerateProxy(
                typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor, fakeCallProcessorProvider);
        }

        public virtual bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason)
        {
            Guard.AgainstNull(method, nameof(method));

            if (method.Name != "Invoke")
            {
                failReason = "Only the Invoke method can be intercepted on delegates.";
                return false;
            }

            failReason = null;
            return true;
        }

        private static Delegate CreateDelegateProxy(
            Type typeOfProxy, MethodInfo invokeMethod, DelegateCallInterceptedEventRaiser eventRaiser)
        {
            var parameterExpressions =
                invokeMethod.GetParameters().Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();

            var body = CreateBodyExpression(invokeMethod, eventRaiser, parameterExpressions);
            return Expression.Lambda(typeOfProxy, body, parameterExpressions).Compile();
        }

        // Generate a method that:
        // - wraps its arguments in an object array
        // - passes this array to eventRaiser.Raise()
        // - assigns the output values back to the ref/out parameters
        // - casts and returns the result of eventRaiser.Raise()
        //
        // For instance, for a delegate like this:
        //
        // delegate int Foo(int x, ref int y, out int z);
        //
        // We generate a method like this:
        //
        // int ProxyFoo(int x, ref int y, out int z)
        // {
        //     var arguments = new[]{ (object)x, (object)y, (object)z };
        //     var result = (int)eventRaiser.Raise(arguments);
        //     y = (int)arguments[1];
        //     z = (int)arguments[2];
        //     return result;
        // }
        //
        // Or, for a delegate with void return type:
        //
        // delegate void Foo(int x, ref int y, out int z);
        //
        // void ProxyFoo(int x, ref int y, out int z)
        // {
        //     var arguments = new[]{ (object)x, (object)y, (object)z };
        //     eventRaiser.Raise(arguments);
        //     y = (int)arguments[1];
        //     z = (int)arguments[2];
        // }
        private static Expression CreateBodyExpression(
            MethodInfo delegateMethod,
            DelegateCallInterceptedEventRaiser eventRaiser,
            ParameterExpression[] parameterExpressions)
        {
            bool isVoid = delegateMethod.ReturnType == typeof(void);

            // Local variables of the generated method
            var arguments = Expression.Variable(typeof(object[]), "arguments");
            var result = isVoid ? null : Expression.Variable(delegateMethod.ReturnType, "result");

            var bodyExpressions = new List<Expression>();

            bodyExpressions.Add(Expression.Assign(arguments, WrapParametersInObjectArray(parameterExpressions)));

            Expression call = Expression.Call(
                Expression.Constant(eventRaiser), DelegateCallInterceptedEventRaiser.RaiseMethod, arguments);

            if (!isVoid)
            {
                // If the return type is non void, cast the result of eventRaiser.Raise()
                // to the real return type and assign to the result variable
                call = Expression.Assign(result, Expression.Convert(call, delegateMethod.ReturnType));
            }

            bodyExpressions.Add(call);

            // After the call, copy the values back to the ref/out parameters
            for (int index = 0; index < parameterExpressions.Length; index++)
            {
                var parameter = parameterExpressions[index];
                if (parameter.IsByRef)
                {
                    var assignment = AssignParameterFromArrayElement(arguments, index, parameter);
                    bodyExpressions.Add(assignment);
                }
            }

            // Return the result if the return type is non-void
            if (!isVoid)
            {
                bodyExpressions.Add(result);
            }

            var variables = isVoid ? new[] { arguments } : new[] { arguments, result };
            return Expression.Block(variables, bodyExpressions);
        }

        private static BinaryExpression AssignParameterFromArrayElement(
            ParameterExpression arguments, int index, ParameterExpression parameter)
        {
            return Expression.Assign(
                parameter,
                Expression.Convert(Expression.ArrayAccess(arguments, Expression.Constant(index)), parameter.Type));
        }

        private static NewArrayExpression WrapParametersInObjectArray(ParameterExpression[] parameterExpressions)
        {
            return Expression.NewArrayInit(
                typeof(object), parameterExpressions.Select(x => Expression.Convert(x, typeof(object))));
        }

        private class DelegateCallInterceptedEventRaiser
        {
            public static readonly MethodInfo RaiseMethod = typeof(DelegateCallInterceptedEventRaiser).GetMethod(nameof(Raise));

            private readonly IFakeCallProcessorProvider fakeCallProcessorProvider;
            private readonly MethodInfo method;

            public DelegateCallInterceptedEventRaiser(IFakeCallProcessorProvider fakeCallProcessorProvider, MethodInfo method, Type type)
            {
                this.fakeCallProcessorProvider = fakeCallProcessorProvider;
                this.method = method;
                this.Instance = CreateDelegateProxy(type, method, this);
            }

            public Delegate Instance { get; }

            public object Raise(object[] arguments)
            {
                var call = new DelegateFakeObjectCall(this.Instance, this.method, arguments);
                this.fakeCallProcessorProvider.Fetch(this.Instance).Process(call);
                return call.ReturnValue;
            }
        }

        private class DelegateFakeObjectCall : IInterceptedFakeObjectCall
        {
            private readonly object[] originalArguments;

            public DelegateFakeObjectCall(Delegate instance, MethodInfo method, object[] arguments)
            {
                this.FakedObject = instance;
                this.originalArguments = arguments.ToArray();
                this.Arguments = new ArgumentCollection(arguments, method);
                this.Method = method;
            }

            public object ReturnValue { get; private set; }

            public MethodInfo Method { get; }

            public ArgumentCollection Arguments { get; }

            public object FakedObject { get; }

            public void SetReturnValue(object value)
            {
                this.ReturnValue = value;
            }

            public void CallBaseMethod()
            {
                throw new NotSupportedException("Can not configure a delegate proxy to call base method.");
            }

            public void SetArgumentValue(int index, object value)
            {
                this.Arguments.GetUnderlyingArgumentsArray()[index] = value;
            }

            public ICompletedFakeObjectCall AsReadOnly()
            {
                return new CompletedFakeObjectCall(
                    this,
                    this.originalArguments,
                    this.ReturnValue);
            }
        }
    }
}
