namespace FakeItEasy.Configuration;

using System;
using System.Diagnostics.CodeAnalysis;
using FakeItEasy.Core;

/// <summary>
/// Configuration that lets the developer specify that an exception should be
/// thrown by a fake object call.
/// </summary>
/// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
public partial interface IExceptionThrowerConfiguration<out TInterface> : IHideObjectMembers
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
    /// <typeparam name="T">The type of exception to throw.</typeparam>
    /// <returns>Configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
    IAfterCallConfiguredConfiguration<TInterface> Throws<T>() where T : Exception, new();
}
