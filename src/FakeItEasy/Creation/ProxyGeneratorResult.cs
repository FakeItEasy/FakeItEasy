namespace FakeItEasy.Creation
{
    using System;
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
            Guard.AgainstNull(reasonForFailure, nameof(reasonForFailure));

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
            Guard.AgainstNull(reasonForFailure, nameof(reasonForFailure));
            Guard.AgainstNull(exception, nameof(exception));

            if (exception is TargetInvocationException && exception.InnerException is object)
            {
                exception = exception.InnerException;
            }

            this.ReasonForFailure =
                string.Format(
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
            Guard.AgainstNull(generatedProxy, nameof(generatedProxy));

            this.GeneratedProxy = generatedProxy;
        }

        /// <summary>
        /// Gets a value indicating whether the proxy was successfully created.
        /// </summary>
        public bool ProxyWasSuccessfullyGenerated => this.GeneratedProxy is object;

        /// <summary>
        /// Gets the generated proxy when it was successfully created.
        /// </summary>
        public object? GeneratedProxy { get; }

        /// <summary>
        /// Gets the reason for failure when the generation was not successful.
        /// </summary>
        public string? ReasonForFailure { get; }
    }
}
