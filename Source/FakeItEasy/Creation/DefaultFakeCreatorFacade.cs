namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;

    /// <summary>
    /// Default implementation ofthe IFakeCreator-interface.
    /// </summary>
    internal class DefaultFakeCreatorFacade
        : IFakeCreatorFacade
    {
        private IFakeAndDummyManager fakeAndDummyManager;

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
        /// <param name="options">Options for the created fake object.</param>
        /// <returns>The created fake object.</returns>
        /// <exception cref="FakeCreationException">Was unable to generate the fake in the current configuration.</exception>
        public T CreateFake<T>(Action<IFakeOptionsBuilder<T>> options)
        {
            Guard.AgainstNull(options, "options");

            var fakeOptions = BuildFakeOptions<T>(options);

            return (T)this.fakeAndDummyManager.CreateFake(typeof(T), fakeOptions);
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

            for (int i = 0; i < numberOfFakes; i++)
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
        /// no dummy was registered in the container for the specifed type..</exception>
        public T CreateDummy<T>()
        {
            return (T)this.fakeAndDummyManager.CreateDummy(typeof(T));
        }

        private static FakeOptions BuildFakeOptions<T>(Action<IFakeOptionsBuilder<T>> options)
        {
            var builder = new FakeOptionsBuilder<T>();
            options.Invoke(builder);
            return builder.Options;
        }

        private class FakeOptionsBuilder<T>
            : IFakeOptionsBuilderForWrappers<T>
        {
            private List<Type> additionalInterfacesToImpelement;

            public FakeOptionsBuilder()
            {
                this.additionalInterfacesToImpelement = new List<Type>();

                this.Options = new FakeOptions();
                this.Options.AdditionalInterfacesToImplement = this.additionalInterfacesToImpelement;
            }

            public FakeOptions Options { get; private set; }

            public IFakeOptionsBuilder<T> WithArgumentsForConstructor(System.Collections.Generic.IEnumerable<object> argumentsForConstructor)
            {
                this.Options.ArgumentsForConstructor = argumentsForConstructor;
                return this;
            }

            public IFakeOptionsBuilder<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall)
            {
                this.Options.ArgumentsForConstructor = GetConstructorArgumentsFromExpression(constructorCall);
                return this;
            }

            public IFakeOptionsBuilderForWrappers<T> Wrapping(T wrappedInstance)
            {
                this.Options.WrappedInstance = wrappedInstance;
                return this;
            }

            public IFakeOptionsBuilder<T> Implements(Type interfaceType)
            {
                this.additionalInterfacesToImpelement.Add(interfaceType);
                return this;
            }

            public IFakeOptionsBuilder<T> RecordedBy(FakeItEasy.SelfInitializedFakes.ISelfInitializingFakeRecorder recorder)
            {
                this.Options.SelfInitializedFakeRecorder = recorder;
                return this;
            }

            private static IEnumerable<object> GetConstructorArgumentsFromExpression(Expression<Func<T>> constructorCall)
            {
                AssertThatExpressionRepresentConstructorCall(constructorCall);

                var constructorArguments =
                    from argument in ((NewExpression)constructorCall.Body).Arguments
                    select Helpers.GetValueProducedByExpression(argument);

                return constructorArguments;
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
