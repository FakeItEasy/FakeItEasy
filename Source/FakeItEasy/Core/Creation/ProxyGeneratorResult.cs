namespace FakeItEasy.Core.Creation
{
    /// <summary>
    /// Contains the result of a call to TryCreateProxy of IProxyGenerator.
    /// </summary>
    public sealed class ProxyGeneratorResult
    {
        /// <summary>
        /// Creates a new instance representing a failed proxy
        /// generation attempt.
        /// </summary>
        /// <param name="reasonForFailure">The reason the proxy generation failed.</param>
        public ProxyGeneratorResult(string reasonForFailure)
        {
            Guard.AgainstNull(reasonForFailure, "reasonForFailure");

            this.ReasonForFailure = reasonForFailure;
        }

        /// <summary>
        /// Creates a new instance representing a successful proxy
        /// generation.
        /// </summary>
        /// <param name="generatedProxy">The proxy that was generated.</param>
        /// <param name="callInterceptedEventRaiser">An event raiser that raises
        /// events when calls are intercepted to the proxy.</param>
        public ProxyGeneratorResult(object generatedProxy, ICallInterceptedEventRaiser callInterceptedEventRaiser)
        {
            Guard.AgainstNull(generatedProxy, "generatedProxy");
            Guard.AgainstNull(callInterceptedEventRaiser, "callInterceptedEventRaiser");

            this.ProxyWasSuccessfullyGenerated = true;
            this.GeneratedProxy = generatedProxy;
            this.CallInterceptedEventRaiser = callInterceptedEventRaiser;
        }

        /// <summary>
        /// Gets a value indicating if the proxy was successfully created.
        /// </summary>
        public bool ProxyWasSuccessfullyGenerated { get; private set; }

        /// <summary>
        /// Gets the generated proxy when it was successfully created.
        /// </summary>
        public object GeneratedProxy { get; private set; }

        /// <summary>
        /// Gets the event raiser that raises events when calls to the proxy are
        /// intercepted.
        /// </summary>
        public ICallInterceptedEventRaiser CallInterceptedEventRaiser { get; private set; }


        /// <summary>
        /// Gets the reason for failure when the generation was not successful.
        /// </summary>
        public string ReasonForFailure { get; private set; }
    }
}
