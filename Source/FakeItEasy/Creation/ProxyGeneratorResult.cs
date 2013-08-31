namespace FakeItEasy.Creation
{
    using System;

    /// <summary>
    /// Contains the result of a call to TryCreateProxy of IProxyGenerator.
    /// </summary>
    public sealed class ProxyGeneratorResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyGeneratorResult"/> class. 
        /// Creates a new instance representing a failed proxy
        /// generation attempt.
        /// </summary>
        /// <param name="reasonForFailure">
        /// The reason the proxy generation failed.
        /// </param>
        public ProxyGeneratorResult(string reasonForFailure)
        {
            Guard.AgainstNull(reasonForFailure, "reasonForFailure");

            this.ReasonForFailure = reasonForFailure;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyGeneratorResult"/> class. 
        /// Creates a new instance representing a failed proxy
        /// generation attempt due to an exception being caught.
        /// </summary>
        /// <param name="reasonForFailure">
        /// The reason the proxy generation failed.
        /// </param>
        /// <param name="exception">
        /// The exception thrown from the creation attempt.
        /// </param>
        public ProxyGeneratorResult(string reasonForFailure, Exception exception)
        {
            Guard.AgainstNull(reasonForFailure, "reasonForFailure");
            Guard.AgainstNull(exception, "exception");

            this.ReasonForFailure = reasonForFailure + System.Environment.NewLine +
                "An exception was caught during this call. Its message was:" + System.Environment.NewLine +
                exception.Message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyGeneratorResult"/> class. 
        /// Creates a new instance representing a successful proxy
        /// generation.
        /// </summary>
        /// <param name="generatedProxy">
        /// The proxy that was generated.
        /// </param>
        /// <param name="callInterceptedEventRaiser">
        /// An event raiser that raises
        /// events when calls are intercepted to the proxy.
        /// </param>
        public ProxyGeneratorResult(object generatedProxy, ICallInterceptedEventRaiser callInterceptedEventRaiser)
        {
            Guard.AgainstNull(generatedProxy, "generatedProxy");
            Guard.AgainstNull(callInterceptedEventRaiser, "callInterceptedEventRaiser");

            this.ProxyWasSuccessfullyGenerated = true;
            this.GeneratedProxy = generatedProxy;
            this.CallInterceptedEventRaiser = callInterceptedEventRaiser;
        }

        /// <summary>
        /// Gets a value indicating whether the proxy was successfully created.
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