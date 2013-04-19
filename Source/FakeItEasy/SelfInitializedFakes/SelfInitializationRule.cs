namespace FakeItEasy.SelfInitializedFakes
{
    using FakeItEasy.Core;

    /// <summary>
    /// A call rule use for self initializing fakes, delegates call to
    /// be applied by the recorder.
    /// </summary>
    internal class SelfInitializationRule
        : IFakeObjectCallRule
    {
        private readonly ISelfInitializingFakeRecorder recorder;
        private readonly IFakeObjectCallRule wrappedRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfInitializationRule"/> class.
        /// </summary>
        /// <param name="wrappedRule">The wrapped rule.</param>
        /// <param name="recorder">The recorder.</param>
        public SelfInitializationRule(IFakeObjectCallRule wrappedRule, ISelfInitializingFakeRecorder recorder)
        {
            this.wrappedRule = wrappedRule;
            this.recorder = recorder;
        }

        /// <summary>
        /// Gets the number of times this call rule is valid, if it's set
        /// to null its infinitely valid.
        /// </summary>
        /// <value></value>
        public int? NumberOfTimesToCall
        {
            get { return this.wrappedRule.NumberOfTimesToCall; }
        }

        /// <summary>
        /// Gets whether this interceptor is applicable to the specified
        /// call, if true is returned the Apply-method of the interceptor will
        /// be called.
        /// </summary>
        /// <param name="fakeObjectCall">The call to check for applicability.</param>
        /// <returns>True if the interceptor is applicable.</returns>
        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return this.wrappedRule.IsApplicableTo(fakeObjectCall);
        }

        /// <summary>
        /// Applies an action to the call, might set a return value or throw
        /// an exception.
        /// </summary>
        /// <param name="fakeObjectCall">The call to apply the interceptor to.</param>
        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

            if (this.recorder.IsRecording)
            {
                this.wrappedRule.Apply(fakeObjectCall);
                this.recorder.RecordCall(fakeObjectCall.AsReadOnly());
            }
            else
            {
                this.recorder.ApplyNext(fakeObjectCall);
            }
        }
    }
}