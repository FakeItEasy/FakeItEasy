namespace FakeItEasy.Configuration
{
    using System;

    /// <summary>
    /// Configuration that lets the developer specify that an exception should be
    /// thrown by a fake object call.
    /// </summary>
    /// <typeparam name="TFake">The type of the fake.</typeparam>
    public interface IExceptionThrowerConfiguration
            : IHideObjectMembers
    {
        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="exception">The exception to throw.</param>
        /// <returns>Configuration object.</returns>
        IAfterCallSpecifiedConfiguration Throws(Exception exception);
    }
}
