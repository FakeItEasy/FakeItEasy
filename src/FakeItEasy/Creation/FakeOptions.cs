namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;

    internal class FakeOptions<T> : IFakeOptions<T>, IFakeOptions
    {
        private readonly ProxyOptions proxyOptions;

        public FakeOptions(ProxyOptions proxyOptions)
        {
            this.proxyOptions = proxyOptions;
        }

        IFakeOptions IFakeOptions.ConfigureFake(Action<object> action)
        {
            return (IFakeOptions)this.ConfigureFake(fake => action(fake));
        }

        IFakeOptions IFakeOptions.WithAttributes(params Expression<Func<Attribute>>[] attributes)
        {
            return (IFakeOptions)this.WithAttributes(attributes);
        }

        IFakeOptions IFakeOptions.Implements(Type interfaceType)
        {
            return (IFakeOptions)this.Implements(interfaceType);
        }

        IFakeOptions IFakeOptions.Implements<TInterface>()
        {
            return (IFakeOptions)this.Implements<TInterface>();
        }

        public IFakeOptions<T> Implements<TInterface>()
        {
            return this.Implements(typeof(TInterface));
        }

        IFakeOptions IFakeOptions.Wrapping(object wrappedInstance)
        {
            return (IFakeOptions)this.Wrapping((T)wrappedInstance);
        }

        public IFakeOptions<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall)
        {
            return this.WithArgumentsForConstructor(GetConstructorArgumentsFromExpression(constructorCall));
        }

        IFakeOptions IFakeOptions.WithArgumentsForConstructor<TConstructor>(Expression<Func<TConstructor>> constructorCall)
        {
            return (IFakeOptions)this.WithArgumentsForConstructor(GetConstructorArgumentsFromExpression(constructorCall));
        }

        IFakeOptions IFakeOptions.WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
        {
            return (IFakeOptions)this.WithArgumentsForConstructor(argumentsForConstructor);
        }

        public IFakeOptions<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
        {
            this.proxyOptions.ArgumentsForConstructor = argumentsForConstructor;
            return this;
        }

        public IFakeOptions<T> WithAttributes(
            params Expression<Func<Attribute>>[] attributes)
        {
            Guard.AgainstNull(attributes, nameof(attributes));

            foreach (var attribute in attributes)
            {
                this.proxyOptions.AddAttribute(attribute);
            }

            return this;
        }

        public IFakeOptions<T> Wrapping(T wrappedInstance)
        {
            Guard.AgainstNull(wrappedInstance, nameof(wrappedInstance));
            this.ConfigureFake(fake => ConfigureFakeToWrap(fake, wrappedInstance));
            return this;
        }

        public IFakeOptions<T> Implements(Type interfaceType)
        {
            this.proxyOptions.AddInterfaceToImplement(interfaceType);
            return this;
        }

        public IFakeOptions<T> ConfigureFake(Action<T> action)
        {
            this.proxyOptions.AddProxyConfigurationAction(proxy => action((T)proxy));
            return this;
        }

        private static void ConfigureFakeToWrap(object fakedObject, object wrappedObject)
        {
            var manager = Fake.GetFakeManager(fakedObject);
            var rule = new WrappedObjectRule(wrappedObject);
            manager.AddRuleFirst(rule);
        }

        private static IEnumerable<object> GetConstructorArgumentsFromExpression<TConstructor>(Expression<Func<TConstructor>> constructorCall)
        {
            AssertThatExpressionRepresentConstructorCall(constructorCall);
            return ((NewExpression)constructorCall.Body).Arguments.Select(argument => argument.Evaluate());
        }

        private static void AssertThatExpressionRepresentConstructorCall<TConstructor>(Expression<Func<TConstructor>> constructorCall)
        {
            if (constructorCall.Body.NodeType != ExpressionType.New)
            {
                throw new ArgumentException(ExceptionMessages.NonConstructorExpression);
            }

            if (typeof(TConstructor) != typeof(T))
            {
                throw new ArgumentException(
                    ExceptionMessages.WrongConstructorExpressionType(
                        typeof(T),
                        typeof(TConstructor)));
            }
        }
    }
}
