namespace FakeItEasy.Core
{
    using FakeItEasy.SelfInitializedFakes;

    /// <summary>
    /// Handles configuring of fake objects to delegate all their calls to a wrapped instance.
    /// </summary>
    internal class DefaultFakeWrapperConfigurer
        : IFakeWrapperConfigurer
    {
        /// <summary>
        /// Configures the specified faked object to wrap the specified instance.
        /// </summary>
        /// <param name="fakedObject">The faked object to configure.</param>
        /// <param name="wrappedInstance">The instance to wrap.</param>
        /// <param name="recorder">The recorder to use, null if no recording should be made.</param>
        public void ConfigureFakeToWrap(object fakedObject, object wrappedInstance, ISelfInitializingFakeRecorder recorder)
        {
            var fake = Fake.GetFakeManager(fakedObject);

            var wrapperRule = CreateAndAddWrapperRule(wrappedInstance, fake);

            AddRecordingRuleWhenRecorderIsSpecified(recorder, fake, wrapperRule);
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
