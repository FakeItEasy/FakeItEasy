namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Expressions;
    using FakeItEasy.SelfInitializedFakes;

    /// <summary>
    /// Default implementation of the IFakeObjectBuilder interface.
    /// </summary>
    internal class FakeObjectBuilder
        : IFakeObjectBuilder
    {
        private FakeObjectFactory fakeObjectFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeObjectBuilder"/> class.
        /// </summary>
        /// <param name="fakeObjectFactory">The fake object factory.</param>
        public FakeObjectBuilder(FakeObjectFactory fakeObjectFactory)
        {
            this.fakeObjectFactory = fakeObjectFactory;
        }

        /// <summary>
        /// Generates a fake object according to the options.
        /// </summary>
        /// <typeparam name="T">The type of fake to generate.</typeparam>
        /// <param name="options">Options for the generation of the fake object.</param>
        /// <returns>A fake object.</returns>
        public T GenerateFake<T>(Action<IFakeOptionsBuilder<T>> options)
        {

            Guard.IsNotNull(options, "options");

            var builtOptions = GetOptions<T>(options);

            T result = this.CreateFake<T>(builtOptions);

            ConfigureFakeAccordingToOptions<T>(builtOptions, result);

            return result;
        }

        private static FakeGenerationOptions GetOptions<T>(Action<IFakeOptionsBuilder<T>> options)
        {
            var builder = new OptionBuilder<T>();

            options(builder);

            return builder.Options;
        }

        private static void ConfigureFakeAccordingToOptions<T>(FakeGenerationOptions options, T result)
        {
            if (options.WrappedInstance != null)
            {
                ConfigureFakeAsWrapper<T>(options, result);
            }
        }

        private static void ConfigureFakeAsWrapper<T>(FakeGenerationOptions options, T result)
        {
            var fakeObject = FakeItEasy.Fake.GetFakeObject(result);
            var wrappedRule = new WrappedObjectRule(options.WrappedInstance);

            if (options.Recorder != null)
            {
                fakeObject.AddRule(new SelfInitializationRule(wrappedRule, options.Recorder));
            }
            else
            {
                fakeObject.AddRule(wrappedRule);
            }
        }

        private T CreateFake<T>(FakeGenerationOptions options)
        {
            if (options.ArgumentsForConstructor != null)
            {
                return CreateFake<T>(options.ArgumentsForConstructor);
            }
            else
            {
                return CreateFake<T>((IEnumerable<object>)null);
            }
        }

        private T CreateFake<T>(IEnumerable<object> argumentsForConstructor)
        {
            return (T)this.fakeObjectFactory.CreateFake(typeof(T), argumentsForConstructor, false);
        }

        private class OptionBuilder<T> : IFakeOptionsBuilderForWrappers<T>
        {
            public OptionBuilder()
            {
                this.Options = new FakeGenerationOptions();
            }

            public FakeGenerationOptions Options { get; private set; }

            public IFakeOptionsBuilder<T> Implementing<TInterface>()
            {
                this.Options.Interfaces.Add(typeof(T));
                return this;
            }

            public IFakeOptionsBuilder<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
            {
                if (!typeof(T).IsAbstract)
                {
                    throw new InvalidOperationException(ExceptionMessages.FakingNonAbstractClassWithArgumentsForConstructor);
                }

                this.Options.ArgumentsForConstructor = argumentsForConstructor;
                return this;
            }

            public IFakeOptionsBuilder<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall)
            {
                this.Options.ArgumentsForConstructor = this.GetConstructorArgumentsFromExpression(constructorCall);
                return this;
            }

            public IFakeOptionsBuilderForWrappers<T> Wrapping(T wrappedInstance)
            {
                this.Options.WrappedInstance = wrappedInstance;
                return this;
            }

            public IFakeOptionsBuilder<T> RecordedBy(ISelfInitializingFakeRecorder recorder)
            {
                this.Options.Recorder = recorder;
                return this;
            }

            private IEnumerable<object> GetConstructorArgumentsFromExpression(Expression<Func<T>> constructorCall)
            {
                if (constructorCall.Body.NodeType != ExpressionType.New)
                {
                    throw new ArgumentException(ExceptionMessages.NonConstructorExpressionMessage);
                }

                var constructorArguments =
                    from argument in ((NewExpression)constructorCall.Body).Arguments
                    select ExpressionManager.GetValueProducedByExpression(argument);

                return constructorArguments;
            }


            public IFakeOptionsBuilder<T> Implements(Type interfaceType)
            {
                throw new NotImplementedException();
            }
        }

        private class FakeGenerationOptions
        {
            public ICollection<Type> Interfaces { get; private set; }
            public IEnumerable<object> ArgumentsForConstructor { get; set; }
            public object WrappedInstance { get; set; }
            public ISelfInitializingFakeRecorder Recorder { get; set; }
        }
    }
}
