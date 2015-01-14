namespace FakeItEasy.Creation.DelegateProxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    internal class DelegateProxyGenerator
        : IProxyGenerator
    {
        public virtual ProxyGeneratorResult GenerateProxy(
            Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement, IEnumerable<object> argumentsForConstructor, IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            Guard.AgainstNull(typeOfProxy, "typeOfProxy");

            if (!typeof(Delegate).IsAssignableFrom(typeOfProxy))
            {
                return
                    new ProxyGeneratorResult("The delegate proxy generator can only create proxies for delegate types.");
            }

            var invokeMethod = typeOfProxy.GetMethod("Invoke");

            var eventRaiser = new DelegateCallInterceptedEventRaiser(fakeCallProcessorProvider);

            var proxy = CreateDelegateProxy(typeOfProxy, invokeMethod, eventRaiser);

            eventRaiser.Method = invokeMethod;
            eventRaiser.Instance = proxy;

            fakeCallProcessorProvider.EnsureInitialized(proxy);

            return new ProxyGeneratorResult(proxy);
        }

        public virtual ProxyGeneratorResult GenerateProxy(
            Type typeOfProxy,
            IEnumerable<Type> additionalInterfacesToImplement,
            IEnumerable<object> argumentsForConstructor,
            IEnumerable<CustomAttributeBuilder> customAttributeBuilders,
            IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            return this.GenerateProxy(typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor, fakeCallProcessorProvider);
        }

        public virtual bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason)
        {
            Guard.AgainstNull(method, "method");

            if (method.Name != "Invoke")
            {
                failReason = "Only the Invoke method can be intercepted on delegates.";
                return false;
            }

            failReason = null;
            return true;
        }

        private static Delegate CreateDelegateProxy(Type typeOfProxy, MethodInfo invokeMethod, DelegateCallInterceptedEventRaiser eventRaiser)
        {
            var delegateMethod = invokeMethod;

            var parameterExpressions =
                delegateMethod.GetParameters().Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();

            var body = CreateBodyExpression(delegateMethod, eventRaiser, parameterExpressions);

            return Expression.Lambda(typeOfProxy, body, parameterExpressions).Compile();
        }

        private static Expression CreateBodyExpression(
            MethodInfo delegateMethod, DelegateCallInterceptedEventRaiser eventRaiser, IEnumerable<Expression> parameterExpressions)
        {
            var parameterExpressionsCastToObject =
                parameterExpressions.Select(x => Expression.Convert(x, typeof(object))).Cast<Expression>().ToArray();

            Expression body = Expression.Call(
                Expression.Constant(eventRaiser),
                DelegateCallInterceptedEventRaiser.RaiseMethod,
                new Expression[] { Expression.NewArrayInit(typeof(object), parameterExpressionsCastToObject) });

            if (!delegateMethod.ReturnType.Equals(typeof(void)))
            {
                body = Expression.Convert(body, delegateMethod.ReturnType);
            }

            return body;
        }

        private class DelegateCallInterceptedEventRaiser
        {
            public static readonly MethodInfo RaiseMethod =
                typeof(DelegateCallInterceptedEventRaiser).GetMethod("Raise");

            private readonly IFakeCallProcessorProvider fakeCallProcessorProvider;

            public DelegateCallInterceptedEventRaiser(IFakeCallProcessorProvider fakeCallProcessorProvider)
            {
                this.fakeCallProcessorProvider = fakeCallProcessorProvider;
            }

            public MethodInfo Method { private get; set; }

            public Delegate Instance { private get; set; }

            // ReSharper disable UnusedMember.Local
            public object Raise(object[] arguments)
            // ReSharper restore UnusedMember.Local
            {
                var call = new DelegateFakeObjectCall(this.Instance, this.Method, arguments);

                this.fakeCallProcessorProvider.Fetch(this.Instance).Process(call);

                return call.ReturnValue;
            }
        }

        private class DelegateFakeObjectCall
            : IWritableFakeObjectCall, ICompletedFakeObjectCall
        {
            private readonly Delegate instance;

            public DelegateFakeObjectCall(Delegate instance, MethodInfo method, object[] arguments)
            {
                this.instance = instance;
                this.Arguments = new ArgumentCollection(arguments, method);
                this.Method = method;
            }

            public object ReturnValue { get; private set; }

            public MethodInfo Method { get; private set; }

            public ArgumentCollection Arguments { get; private set; }

            public object FakedObject
            {
                get { return this.instance; }
            }

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
                return this;
            }
        }
    }
}