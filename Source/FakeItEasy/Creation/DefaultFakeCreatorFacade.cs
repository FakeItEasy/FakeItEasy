namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection.Emit;
    using FakeItEasy.Core;

    /// <summary>
    /// Default implementation of the IFakeCreator-interface.
    /// </summary>
    internal class DefaultFakeCreatorFacade
        : IFakeCreatorFacade
    {
        private readonly IFakeAndDummyManager fakeAndDummyManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFakeCreatorFacade"/> class.
        /// </summary>
        /// <param name="fakeAndDummyManager">The fake and dummy manager.</param>
        public DefaultFakeCreatorFacade(IFakeAndDummyManager fakeAndDummyManager)
        {
            this.fakeAndDummyManager = fakeAndDummyManager;
        }

        /// <summary>
        /// Creates a fake object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fake to create.</typeparam>
        /// <param name="optionsBuilder">Action that builds options for the created fake object.</param>
        /// <returns>The created fake object.</returns>
        /// <exception cref="FakeCreationException">Was unable to generate the fake in the current configuration.</exception>
        public T CreateFake<T>(Action<IFakeOptions<T>> optionsBuilder)
        {
            Guard.AgainstNull(optionsBuilder, "optionsBuilder");

            var proxyOptions = BuildProxyOptions(optionsBuilder);

            return (T)this.fakeAndDummyManager.CreateFake(typeof(T), proxyOptions);
        }

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fakes to create.</typeparam>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <returns>
        /// A collection of fake objects of the specified type.
        /// </returns>
        public IList<T> CollectionOfFake<T>(int numberOfFakes)
        {
            var result = new List<T>();

            for (var i = 0; i < numberOfFakes; i++)
            {
                result.Add(this.CreateFake<T>(x => { }));
            }

            return result;
        }

        /// <summary>
        /// Creates a dummy object, this can be a fake object or an object resolved
        /// from the current IFakeObjectContainer.
        /// </summary>
        /// <typeparam name="T">The type of dummy to create.</typeparam>
        /// <returns>The created dummy.</returns>
        /// <exception cref="FakeCreationException">Was unable to generate the fake in the current configuration and
        /// no dummy was registered in the container for the specified type..</exception>
        public T CreateDummy<T>()
        {
            return (T)this.fakeAndDummyManager.CreateDummy(typeof(T));
        }

        private static IProxyOptions BuildProxyOptions<T>(Action<IFakeOptions<T>> optionsBuilder)
        {
            var fakeOptions = new FakeOptions<T>();
            optionsBuilder.Invoke(fakeOptions);
            return fakeOptions.ProxyOptions;
        }

        private class FakeOptions<T>
            : IFakeOptions<T>
        {
            private readonly ProxyOptions proxyOptions;

            public FakeOptions()
            {
                this.proxyOptions = new ProxyOptions();
            }

            public IProxyOptions ProxyOptions
            {
                get { return this.proxyOptions; }
            }

            public IFakeOptions<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
            {
                this.proxyOptions.ArgumentsForConstructor = argumentsForConstructor;
                return this;
            }

            public IFakeOptions<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall)
            {
                this.proxyOptions.ArgumentsForConstructor = GetConstructorArgumentsFromExpression(constructorCall);
                return this;
            }

            public IFakeOptions<T> WithAdditionalAttributes(
                IEnumerable<CustomAttributeBuilder> customAttributeBuilders)
            {
                Guard.AgainstNull(customAttributeBuilders, "customAttributeBuilders");

                foreach (var customAttributeBuilder in customAttributeBuilders)
                {
                    this.proxyOptions.AddAttribute(customAttributeBuilder);
                }

                return this;
            }

            public IFakeOptionsForWrappers<T> Wrapping(T wrappedInstance)
            {
                var wrapper = new FakeWrapperConfigurator<T>(this, wrappedInstance);
                this.ConfigureFake(fake => wrapper.ConfigureFakeToWrap(fake));
                return wrapper;
            }

            public IFakeOptions<T> Implements(Type interfaceType)
            {
                this.proxyOptions.AddInterfaceToImplement(interfaceType);
                return this;
            }

            public IFakeOptions<T> Implements<TInterface>()
            {
                return this.Implements(typeof(TInterface));
            }

            public IFakeOptions<T> ConfigureFake(Action<T> action)
            {
                this.proxyOptions.AddProxyConfigurationAction(proxy => action((T)proxy));
                return this;
            }

            private static IEnumerable<object> GetConstructorArgumentsFromExpression(Expression<Func<T>> constructorCall)
            {
                AssertThatExpressionRepresentConstructorCall(constructorCall);
                return ((NewExpression)constructorCall.Body).Arguments.Select(argument => argument.Evaluate());
            }

            private static void AssertThatExpressionRepresentConstructorCall(Expression<Func<T>> constructorCall)
            {
                if (constructorCall.Body.NodeType != ExpressionType.New)
                {
                    throw new ArgumentException(ExceptionMessages.NonConstructorExpressionMessage);
                }
            }
        }
    }
}