namespace FakeItEasy;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FakeItEasy.Configuration;
using FakeItEasy.Core;

/// <summary>
/// Provides extension methods for <see cref="IReturnValueConfiguration{T}"/> to configure async methods to return a failed task.
/// </summary>
public static partial class AsyncReturnValueConfigurationExtensions
{
    /// <summary>
    /// Returns a failed task with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exception">The exception to set on the returned task when a call that matches is invoked.</param>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync(
        this IReturnValueConfiguration<Task> configuration,
        Exception exception)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exception);

        return configuration.ReturnsLazily(call => TaskHelper.FromException(exception));
    }

    /// <summary>
    /// Returns a failed task with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync(
        this IReturnValueConfiguration<Task> configuration,
        Func<IFakeObjectCall, Exception> exceptionFactory)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exceptionFactory);

        return configuration.ReturnsLazily(call => TaskHelper.FromException(exceptionFactory(call)));
    }

    /// <summary>
    /// Returns a failed task with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync(
        this IReturnValueConfiguration<Task> configuration,
        Func<Exception> exceptionFactory)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exceptionFactory);

        return configuration.ReturnsLazily(call => TaskHelper.FromException(exceptionFactory()));
    }

    /// <summary>
    /// Returns a failed task with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exception">The exception to set on the returned task when a call that matches is invoked.</param>
    /// <typeparam name="T">The type of the returned task's result.</typeparam>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T>(
        this IReturnValueConfiguration<Task<T>> configuration,
        Exception exception)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exception);

        return configuration.ReturnsLazily(call => TaskHelper.FromException<T>(exception));
    }

    /// <summary>
    /// Returns a failed task with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
    /// <typeparam name="T">The type of the returned task's result.</typeparam>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T>(
        this IReturnValueConfiguration<Task<T>> configuration,
        Func<IFakeObjectCall, Exception> exceptionFactory)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exceptionFactory);

        return configuration.ReturnsLazily(call => TaskHelper.FromException<T>(exceptionFactory(call)));
    }

    /// <summary>
    /// Returns a failed task with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
    /// <typeparam name="T">The type of the returned task's result.</typeparam>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T>(
        this IReturnValueConfiguration<Task<T>> configuration,
        Func<Exception> exceptionFactory)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exceptionFactory);

        return configuration.ReturnsLazily(call => TaskHelper.FromException<T>(exceptionFactory()));
    }
}
