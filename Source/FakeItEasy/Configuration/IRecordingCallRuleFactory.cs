namespace FakeItEasy.Configuration
{
    using FakeItEasy.Core;

    /// <summary>
    /// A factory that creates instances of the RecordingCallRuleType.
    /// </summary>
    internal interface IRecordingCallRuleFactory
    {
        /// <summary>
        /// Creates the specified fake object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="fakeObject">The fake object the rule belongs to.</param>
        /// <param name="recordedRule">The rule that's being recorded.</param>
        /// <returns>A RecordingCallRule instance.</returns>
        RecordingCallRule<TFake> Create<TFake>(FakeManager fakeObject, RecordedCallRule recordedRule);
    }
}