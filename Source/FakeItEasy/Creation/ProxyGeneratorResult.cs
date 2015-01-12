namespace FakeItEasy.Creation
{
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Contains the result of a call to TryCreateProxy of IProxyGenerator.
    /// </summary>
    internal sealed class ProxyGeneratorResult
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

            if (exception is TargetInvocationException && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            this.ReasonForFailure =
                string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}{1}An exception of type {2} was caught during this call. Its message was:{1}{3}{1}{4}",
                    reasonForFailure,
                    Environment.NewLine,
                    exception.GetType(),
                    exception.Message,
                    exception.StackTrace);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyGeneratorResult"/> class. 
        /// Creates a new instance representing a successful proxy
        /// generation.
        /// </summary>
        /// <param name="generatedProxy">
        /// The proxy that was generated.
        /// </param>
        public ProxyGeneratorResult(object generatedProxy)
        {
            Guard.AgainstNull(generatedProxy, "generatedProxy");

            this.ProxyWasSuccessfullyGenerated = true;
            this.GeneratedProxy = generatedProxy;
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
        /// Gets the reason for failure when the generation was not successful.
        /// </summary>
        public string ReasonForFailure { get; private set; }
    }
}