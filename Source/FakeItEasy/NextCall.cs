namespace FakeItEasy
{
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    /// <summary>
    /// Lets you specify options for the next call to a fake object.
    /// </summary>
    public static class NextCall
    {
        /// <summary>
        /// Specifies options for the next call to the specified fake object. The next call will
        /// be recorded as a call configuration.
        /// </summary>
        /// <typeparam name="TFake">The type of the faked object.</typeparam>
        /// <param name="fake">The faked object to configure.</param>
        /// <returns>A call configuration object.</returns>
        public static IRecordingConfigurationWithArgumentValidation To<TFake>(TFake fake)
        {
            Guard.AgainstNull(fake, "fake");

            var recordedRule = CreateRecordedRule();
            var fakeManager = Fake.GetFakeManager(fake);
            var recordingRule = CreateRecordingRule<TFake>(recordedRule, fakeManager);

            fakeManager.AddRuleFirst(recordingRule);

            return CreateBuilder(recordedRule, fakeManager);
        }

        private static RecordingRuleBuilder CreateBuilder(RecordedCallRule rule, FakeManager fakeObject)
        {
            var factory = ServiceLocator.Current.Resolve<RecordingRuleBuilder.Factory>();
            return factory.Invoke(rule, fakeObject);
        }

        private static RecordingCallRule<TFake> CreateRecordingRule<TFake>(RecordedCallRule recordedRule, FakeManager fakeObject)
        {
            var factory = ServiceLocator.Current.Resolve<IRecordingCallRuleFactory>();
            return factory.Create<TFake>(fakeObject, recordedRule);
        }

        private static RecordedCallRule CreateRecordedRule()
        {
            return ServiceLocator.Current.Resolve<RecordedCallRule.Factory>().Invoke();
        }
    }
}