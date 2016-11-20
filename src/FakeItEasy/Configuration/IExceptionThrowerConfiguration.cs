namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    /// <summary>
    /// Configuration that lets the developer specify that an exception should be
    /// thrown by a fake object call.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IExceptionThrowerConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="exceptionFactory">A function that creates the exception to throw.</param>
        /// <returns>Configuration object.</returns>
        IAfterCallConfiguredConfiguration<TInterface> Throws(Func<IFakeObjectCall, Exception> exceptionFactory);

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        IAfterCallConfiguredConfiguration<TInterface> Throws<T1>(Func<T1, Exception> exceptionFactory);

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        IAfterCallConfiguredConfiguration<TInterface> Throws<T1, T2>(Func<T1, T2, Exception> exceptionFactory);

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        IAfterCallConfiguredConfiguration<TInterface> Throws<T1, T2, T3>(Func<T1, T2, T3, Exception> exceptionFactory);

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        IAfterCallConfiguredConfiguration<TInterface> Throws<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Exception> exceptionFactory);

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <typeparam name="T">The type of exception to throw.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
        IAfterCallConfiguredConfiguration<TInterface> Throws<T>() where T : Exception, new();
    }
}
