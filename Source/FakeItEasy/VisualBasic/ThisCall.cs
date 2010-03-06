namespace FakeItEasy.VisualBasic
{
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    /// <summary>
    /// Lets you specify options for the next call to a fake object.
    /// </summary>
    public static class ThisCall
    {
        /// <summary>
        /// Specifies options for the next call to the specified fake object. The next call will
        /// be recorded as a call configuration.
        /// </summary>
        /// <typeparam name="TFake">The type of the faked object.</typeparam>
        /// <param name="fake">The faked object to configure.</param>
        /// <returns>A call configuration object.</returns>
        public static IVisualBasicConfigurationWithArgumentValidation To<TFake>(TFake fake)
        {
            Guard.IsNotNull(fake, "fake");

            var recordedRule = CreateRecordedRule();
            var fakeObject = Fake.GetFakeObject(fake);
            var recordingRule = CreateRecordingRule<TFake>(recordedRule, fakeObject);

            fakeObject.AddRule(recordingRule);

            return CreateBuilder(recordedRule, fakeObject);
        }

        private static RuleBuilder CreateBuilder(RecordedCallRule rule, FakeObject fakeObject)
        {
            var factory = ServiceLocator.Current.Resolve<RuleBuilder.Factory>();
            return factory.Invoke(rule, fakeObject);
        }

        private static RecordingCallRule<TFake> CreateRecordingRule<TFake>(RecordedCallRule recordedRule, FakeObject fakeObject)
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