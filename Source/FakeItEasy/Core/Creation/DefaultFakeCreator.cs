namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Default implementation ofthe IFakeCreator-interface.
    /// </summary>
    internal class DefaultFakeCreator
        : IFakeCreator
    {
        private IFakeAndDummyManager fakeAndDummyManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFakeCreator"/> class.
        /// </summary>
        /// <param name="fakeAndDummyManager">The fake and dummy manager.</param>
        public DefaultFakeCreator(IFakeAndDummyManager fakeAndDummyManager)
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
            var fakeOptions = BuildFakeOptions<T>(options);

            this.fakeAndDummyManager.CreateFake(typeof(T), fakeOptions);

            return default(T);
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
            throw new NotImplementedException();
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
            public FakeOptionsBuilder()
            {
                this.Options = new FakeOptions();
            }

            public FakeOptions Options { get; private set; }

            public IFakeOptionsBuilder<T> WithArgumentsForConstructor(System.Collections.Generic.IEnumerable<object> argumentsForConstructor)
            {
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
                return this;
            }

            public IFakeOptionsBuilder<T> RecordedBy(FakeItEasy.SelfInitializedFakes.ISelfInitializingFakeRecorder recorder)
            {
                return this;
            }

            private static IEnumerable<object> GetConstructorArgumentsFromExpression(Expression<Func<T>> constructorCall)
            {
                AssertThatExpressionRepresentConstructorCall(constructorCall);

                var constructorArguments =
                    from argument in ((NewExpression)constructorCall.Body).Arguments
                    select ExpressionManager.GetValueProducedByExpression(argument);

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
