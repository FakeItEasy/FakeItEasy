namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FakeItEasy.Creation;
#if FEATURE_SELF_INITIALIZED_FAKES
    using FakeItEasy.SelfInitializedFakes;
#endif

    /// <summary>
    /// Handles configuring of fake objects to delegate all their calls to a wrapped instance.
    /// </summary>
    /// <typeparam name="T">The type of fake object generated.</typeparam>
    internal class FakeWrapperConfigurator<T>
#if FEATURE_SELF_INITIALIZED_FAKES
        : FakeOptionsBase<T>, IFakeOptionsForWrappers<T>, IFakeOptionsForWrappers
#else
        : FakeOptionsBase<T>
#endif
    {
        private readonly IFakeOptions<T> fakeOptions;
#if FEATURE_SELF_INITIALIZED_FAKES
#pragma warning disable CS0618 // Type or member is obsolete
        private ISelfInitializingFakeRecorder recorder;
#pragma warning restore CS0618 // Type or member is obsolete
#endif

        public FakeWrapperConfigurator(IFakeOptions<T> fakeOptions, object wrappedObject)
        {
            this.fakeOptions = fakeOptions;
            this.WrappedObject = wrappedObject;
        }

        private object WrappedObject { get; }

        public override IFakeOptions<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
        {
            return this.fakeOptions.WithArgumentsForConstructor(argumentsForConstructor);
        }

#if FEATURE_SELF_INITIALIZED_FAKES
        public override IFakeOptionsForWrappers<T> Wrapping(T wrappedInstance)
#else
        public override IFakeOptions<T> Wrapping(T wrappedInstance)
#endif
        {
            return this.fakeOptions.Wrapping(wrappedInstance);
        }

        public override IFakeOptions<T> WithAttributes(params Expression<Func<Attribute>>[] attributes)
        {
            return this.fakeOptions.WithAttributes(attributes);
        }

        public override IFakeOptions<T> Implements(Type interfaceType)
        {
            return this.fakeOptions.Implements(interfaceType);
        }

        public override IFakeOptions<T> ConfigureFake(Action<T> action)
        {
            return this.fakeOptions.ConfigureFake(action);
        }

#if FEATURE_SELF_INITIALIZED_FAKES
        [Obsolete("Self-initializing fakes will be removed in version 4.0.0.")]
        public IFakeOptions<T> RecordedBy(ISelfInitializingFakeRecorder fakeRecorder)
        {
            this.recorder = fakeRecorder;
            return this.fakeOptions;
        }

        [Obsolete("Self-initializing fakes will be removed in version 4.0.0.")]
        IFakeOptions IFakeOptionsForWrappers.RecordedBy(ISelfInitializingFakeRecorder fakeRecorder)
        {
            return (IFakeOptions)this.RecordedBy(fakeRecorder);
        }
#endif

        /// <summary>
        /// Configures the specified faked object to wrap the specified instance.
        /// </summary>
        /// <param name="fakedObject">The faked object to configure.</param>
        public void ConfigureFakeToWrap(object fakedObject)
        {
            var fake = Fake.GetFakeManager(fakedObject);

            var wrapperRule = CreateAndAddWrapperRule(this.WrappedObject, fake);

#if FEATURE_SELF_INITIALIZED_FAKES
            AddRecordingRuleWhenRecorderIsSpecified(this.recorder, fake, wrapperRule);
#endif
        }

#if FEATURE_SELF_INITIALIZED_FAKES
#pragma warning disable CS0618 // Type or member is obsolete
        private static void AddRecordingRuleWhenRecorderIsSpecified(ISelfInitializingFakeRecorder recorder, FakeManager fake, WrappedObjectRule wrapperRule)
        {
            if (recorder != null)
            {
                fake.AddRuleFirst(new SelfInitializationRule(wrapperRule, recorder));
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete
#endif

        private static WrappedObjectRule CreateAndAddWrapperRule(object wrappedInstance, FakeManager fake)
        {
            var wrappingRule = new WrappedObjectRule(wrappedInstance);
            fake.AddRuleFirst(wrappingRule);
            return wrappingRule;
        }
    }
}
