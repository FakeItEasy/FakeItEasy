namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection.Emit;
    using Creation;
    using FakeItEasy.SelfInitializedFakes;

    /// <summary>
    /// Handles configuring of fake objects to delegate all their calls to a wrapped instance.
    /// </summary>
    /// <typeparam name="T">The type of fake object generated.</typeparam>
    internal class FakeWrapperConfigurator<T> : IFakeOptionsForWrappers<T>
    {
        private readonly IFakeOptions<T> fakeOptions;

        public FakeWrapperConfigurator(IFakeOptions<T> fakeOptions, object wrappedObject)
        {
            this.fakeOptions = fakeOptions;
            this.WrappedObject = wrappedObject;
        }

        public ISelfInitializingFakeRecorder Recorder { private get; set; }

        private object WrappedObject { get; set; }

        public IFakeOptions<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
        {
            return this.fakeOptions.WithArgumentsForConstructor(argumentsForConstructor);
        }

        public IFakeOptions<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall)
        {
            return this.fakeOptions.WithArgumentsForConstructor(constructorCall);
        }

        public IFakeOptionsForWrappers<T> Wrapping(T wrappedInstance)
        {
            return this.fakeOptions.Wrapping(wrappedInstance);
        }

        public IFakeOptions<T> WithAdditionalAttributes(IEnumerable<CustomAttributeBuilder> customAttributeBuilders)
        {
            return this.fakeOptions.WithAdditionalAttributes(customAttributeBuilders);
        }

        public IFakeOptions<T> Implements(Type interfaceType)
        {
            return this.fakeOptions.Implements(interfaceType);
        }

        public IFakeOptions<T> Implements<TInterface>()
        {
            return this.fakeOptions.Implements<TInterface>();
        }

        public IFakeOptions<T> ConfigureFake(Action<T> action)
        {
            return this.fakeOptions.ConfigureFake(action);
        }

        public IFakeOptions<T> RecordedBy(ISelfInitializingFakeRecorder recorder)
        {
            this.Recorder = recorder;
            return this.fakeOptions;
        }

        /// <summary>
        /// Configures the specified faked object to wrap the specified instance.
        /// </summary>
        /// <param name="fakedObject">The faked object to configure.</param>
        public void ConfigureFakeToWrap(object fakedObject)
        {
            var fake = Fake.GetFakeManager(fakedObject);

            var wrapperRule = CreateAndAddWrapperRule(this.WrappedObject, fake);

            AddRecordingRuleWhenRecorderIsSpecified(this.Recorder, fake, wrapperRule);
        }

        private static void AddRecordingRuleWhenRecorderIsSpecified(ISelfInitializingFakeRecorder recorder, FakeManager fake, WrappedObjectRule wrapperRule)
        {
            if (recorder != null)
            {
                fake.AddRuleFirst(new SelfInitializationRule(wrapperRule, recorder));
            }
        }

        private static WrappedObjectRule CreateAndAddWrapperRule(object wrappedInstance, FakeManager fake)
        {
            var wrappingRule = new WrappedObjectRule(wrappedInstance);
            fake.AddRuleFirst(wrappingRule);
            return wrappingRule;
        }
    }
}