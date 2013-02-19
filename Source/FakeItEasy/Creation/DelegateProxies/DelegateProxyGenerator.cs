﻿using System.Reflection.Emit;

namespace FakeItEasy.Creation.DelegateProxies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Core;

    internal class DelegateProxyGenerator
        : IProxyGenerator
    {
        public virtual ProxyGeneratorResult GenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement,
                                                  IEnumerable<object> argumentsForConstructor)
        {
            if (!typeof (Delegate).IsAssignableFrom(typeOfProxy))
            {
                return
                    new ProxyGeneratorResult("The delegate proxy generator can only create proxies for delegate types.");
            }


            var invokeMethod = typeOfProxy.GetMethod("Invoke");

            var eventRaiser = new DelegateCallInterceptedEventRaiser();
            
            var proxy = CreateDelegateProxy(typeOfProxy, invokeMethod, eventRaiser);
            
            eventRaiser.Method = invokeMethod;
            eventRaiser.Instance = proxy;

            return new ProxyGeneratorResult(proxy, eventRaiser);
        }

        public virtual ProxyGeneratorResult GenerateProxy(Type typeOfProxy, IEnumerable<Type> additionalInterfacesToImplement,
                                          IEnumerable<object> argumentsForConstructor, IEnumerable<CustomAttributeBuilder> customAttributeBuilders )
        {
            return GenerateProxy(typeOfProxy, additionalInterfacesToImplement, argumentsForConstructor);
        }

        public virtual bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason)
        {
            if (method.Name != "Invoke")
            {
                failReason = "Only the Invoke method can be intercepted on delegates.";
                return false;
            }

            failReason = null;
            return true;
        }

        static Delegate CreateDelegateProxy(Type typeOfProxy, MethodInfo invokeMethod, DelegateCallInterceptedEventRaiser eventRaiser)
        {
            var delegateMethod = invokeMethod;

            var parameterExpressions =
                delegateMethod.GetParameters().Select(x => Expression.Parameter(x.ParameterType, x.Name)).ToArray();

            var body = CreateBodyExpression(delegateMethod, eventRaiser, parameterExpressions);

            return Expression.Lambda(typeOfProxy,
                                     body,
                                     parameterExpressions).Compile();
        }

        static Expression CreateBodyExpression(MethodInfo delegateMethod, DelegateCallInterceptedEventRaiser eventRaiser, Expression[] parameterExpressions)
        {
            var parameterExpressionsCastToObject =
                parameterExpressions.Select(x => Expression.Convert(x, typeof(object))).Cast<Expression>().ToArray();

            Expression body = Expression.Call(
                Expression.Constant(eventRaiser),
                DelegateCallInterceptedEventRaiser.RaiseMethod,
                new Expression[]
                    {
                        Expression.NewArrayInit(typeof (object), parameterExpressionsCastToObject)
                    });

            if (!delegateMethod.ReturnType.Equals(typeof (void)))
            {
                body = Expression.Convert(body, delegateMethod.ReturnType);
            }

            return body;
        }

        class DelegateCallInterceptedEventRaiser : ICallInterceptedEventRaiser
        {
            public static readonly MethodInfo RaiseMethod =
                typeof (DelegateCallInterceptedEventRaiser).GetMethod("Raise");

            public MethodInfo Method { get; set; }

            public Delegate Instance { get; set; }

            public event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;

// ReSharper disable UnusedMember.Local
            public object Raise(object[] arguments)
// ReSharper restore UnusedMember.Local
            {
                var call = new DelegateFakeObjectCall(this.Instance, this.Method, arguments);
                
                this.RaiseCallWasIntercepted(call);

                return call.ReturnValue;
            }

            private void RaiseCallWasIntercepted(DelegateFakeObjectCall call)
            {
                var handler = this.CallWasIntercepted;

                if (handler != null)
                {
                    this.CallWasIntercepted(null, new CallInterceptedEventArgs(call));
                }
            }
        }

        class DelegateFakeObjectCall
            : IWritableFakeObjectCall, ICompletedFakeObjectCall
        {
            readonly Delegate instance;

            public DelegateFakeObjectCall(Delegate instance, MethodInfo method, object[] arguments)
            {
                this.instance = instance;
                this.Arguments = new ArgumentCollection(arguments, method);
                this.Method = method;
            }

            public object ReturnValue { get; private set; }

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

            public MethodInfo Method { get; private set; }

            public ArgumentCollection Arguments { get; private set; }

            public object FakedObject
            {
                get { return this.instance; }
            }
        }
    }
}