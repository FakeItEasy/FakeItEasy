namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Creation;
    using FakeItEasy.SelfInitializedFakes;

    /// <summary>
    /// Handles configuring of fake objects to delegate all their calls to a wrapped instance.
    /// </summary>
    /// <typeparam name="T">The type of fake object generated.</typeparam>
    internal class FakeWrapperConfigurator<T>
        : FakeOptionsBase<T>, IFakeOptionsForWrappers<T>, IFakeOptionsForWrappers
    {
        private readonly IFakeOptions<T> fakeOptions;
        private ISelfInitializingFakeRecorder recorder;

        public FakeWrapperConfigurator(IFakeOptions<T> fakeOptions, object wrappedObject)
        {
            this.fakeOptions = fakeOptions;
            this.WrappedObject = wrappedObject;
        }

        private object WrappedObject { get; set; }

        public override IFakeOptions<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
        {
            return this.fakeOptions.WithArgumentsForConstructor(argumentsForConstructor);
        }

        public override IFakeOptionsForWrappers<T> Wrapping(T wrappedInstance)
        {
            return this.fakeOptions.Wrapping(wrappedInstance);
        }

        public override IFakeOptions<T> WithAdditionalAttributes(IEnumerable<CustomAttributeBuilder> customAttributeBuilders)
        {
            return this.fakeOptions.WithAdditionalAttributes(customAttributeBuilders);
        }

        public override IFakeOptions<T> Implements(Type interfaceType)
        {
            return this.fakeOptions.Implements(interfaceType);
        }

        public override IFakeOptions<T> ConfigureFake(Action<T> action)
        {
            return this.fakeOptions.ConfigureFake(action);
        }

        public IFakeOptions<T> RecordedBy(ISelfInitializingFakeRecorder fakeRecorder)
        {
            this.recorder = fakeRecorder;
            return this.fakeOptions;
        }

        IFakeOptions IFakeOptionsForWrappers.RecordedBy(ISelfInitializingFakeRecorder fakeRecorder)
        {
            return (IFakeOptions)this.RecordedBy(fakeRecorder);
        }

        /// <summary>
        /// Configures the specified faked object to wrap the specified instance.
        /// </summary>
        /// <param name="fakedObject">The faked object to configure.</param>
        public void ConfigureFakeToWrap(object fakedObject)
        {
            var fake = Fake.GetFakeManager(fakedObject);

            var wrapperRule = CreateAndAddWrapperRule(this.WrappedObject, fake);

            AddRecordingRuleWhenRecorderIsSpecified(this.recorder, fake, wrapperRule);
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
