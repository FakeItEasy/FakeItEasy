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
    internal class FakeWrapperConfigurator<T> : IFakeOptionsBuilderForWrappers<T>
    {
        private readonly IFakeOptionsBuilder<T> fakeOptionsBuilder;

        public FakeWrapperConfigurator(IFakeOptionsBuilder<T> fakeOptionsBuilder, object wrappedObject)
        {
            this.fakeOptionsBuilder = fakeOptionsBuilder;
            this.WrappedObject = wrappedObject;
        }

        public ISelfInitializingFakeRecorder Recorder { private get; set; }

        private object WrappedObject { get; set; }

        public IFakeOptionsBuilder<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
        {
            return this.fakeOptionsBuilder.WithArgumentsForConstructor(argumentsForConstructor);
        }

        public IFakeOptionsBuilder<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall)
        {
            return this.fakeOptionsBuilder.WithArgumentsForConstructor(constructorCall);
        }

        public IFakeOptionsBuilderForWrappers<T> Wrapping(T wrappedInstance)
        {
            return this.fakeOptionsBuilder.Wrapping(wrappedInstance);
        }

        public IFakeOptionsBuilder<T> WithAdditionalAttributes(IEnumerable<CustomAttributeBuilder> customAttributeBuilders)
        {
            return this.fakeOptionsBuilder.WithAdditionalAttributes(customAttributeBuilders);
        }

        public IFakeOptionsBuilder<T> Implements(Type interfaceType)
        {
            return this.fakeOptionsBuilder.Implements(interfaceType);
        }

        public IFakeOptionsBuilder<T> Implements<TInterface>()
        {
            return this.fakeOptionsBuilder.Implements<TInterface>();
        }

        public IFakeOptionsBuilder<T> ConfigureFake(Action<T> action)
        {
            return this.fakeOptionsBuilder.ConfigureFake(action);
        }

        public IFakeOptionsBuilder<T> RecordedBy(ISelfInitializingFakeRecorder recorder)
        {
            this.Recorder = recorder;
            return this.fakeOptionsBuilder;
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