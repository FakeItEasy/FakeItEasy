namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Core;

    /// <summary>
    /// Configuration that lets the developer specify that an exception should be
    /// thrown by a fake object call.
    /// </summary>
    public interface IExceptionThrowerConfiguration
        : IHideObjectMembers
    {
        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="exceptionFactory">A function that creates the exception to throw.</param>
        /// <returns>Configuration object.</returns>
        IAfterCallSpecifiedConfiguration Throws(Func<IFakeObjectCall, Exception> exceptionFactory);
    }
}