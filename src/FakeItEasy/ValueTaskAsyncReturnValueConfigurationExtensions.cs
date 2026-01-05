namespace FakeItEasy;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FakeItEasy.Configuration;
using FakeItEasy.Core;

/// <summary>
/// Provides extension methods for <see cref="IReturnValueConfiguration{T}"/> to configure async methods returning a ValueTask to return a failed ValueTask.
/// </summary>
public static partial class ValueTaskAsyncReturnValueConfigurationExtensions
{
    /// <summary>
    /// Returns a failed ValueTask with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exception">The exception to set on the returned ValueTask when a call that matches is invoked.</param>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<ValueTask>> ThrowsAsync(
        this IReturnValueConfiguration<ValueTask> configuration,
        Exception exception)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exception);

        return configuration.ReturnsLazily(call => new ValueTask(TaskHelper.FromException(exception)));
    }

    /// <summary>
    /// Returns a failed ValueTask with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exceptionFactory">A function that returns the exception to set on the returned ValueTask when a call that matches is invoked.</param>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<ValueTask>> ThrowsAsync(
        this IReturnValueConfiguration<ValueTask> configuration,
        Func<IFakeObjectCall, Exception> exceptionFactory)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exceptionFactory);

        return configuration.ReturnsLazily(call => new ValueTask(TaskHelper.FromException(exceptionFactory(call))));
    }

    /// <summary>
    /// Returns a failed ValueTask with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exceptionFactory">A function that returns the exception to set on the returned ValueTask when a call that matches is invoked.</param>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<ValueTask>> ThrowsAsync(
        this IReturnValueConfiguration<ValueTask> configuration,
        Func<Exception> exceptionFactory)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exceptionFactory);

        return configuration.ReturnsLazily(call => new ValueTask(TaskHelper.FromException(exceptionFactory())));
    }

    /// <summary>
    /// Returns a failed ValueTask with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exception">The exception to set on the returned ValueTask when a call that matches is invoked.</param>
    /// <typeparam name="T">The type of the returned ValueTask's result.</typeparam>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<ValueTask<T>>> ThrowsAsync<T>(
        this IReturnValueConfiguration<ValueTask<T>> configuration,
        Exception exception)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exception);

        return configuration.ReturnsLazily(call => new ValueTask<T>(TaskHelper.FromException<T>(exception)));
    }

    /// <summary>
    /// Returns a failed ValueTask with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exceptionFactory">A function that returns the exception to set on the returned ValueTask when a call that matches is invoked.</param>
    /// <typeparam name="T">The type of the returned ValueTask's result.</typeparam>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<ValueTask<T>>> ThrowsAsync<T>(
        this IReturnValueConfiguration<ValueTask<T>> configuration,
        Func<IFakeObjectCall, Exception> exceptionFactory)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exceptionFactory);

        return configuration.ReturnsLazily(call => new ValueTask<T>(TaskHelper.FromException<T>(exceptionFactory(call))));
    }

    /// <summary>
    /// Returns a failed ValueTask with the specified exception when the currently configured call gets called.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    /// <param name="exceptionFactory">A function that returns the exception to set on the returned ValueTask when a call that matches is invoked.</param>
    /// <typeparam name="T">The type of the returned ValueTask's result.</typeparam>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
    public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<ValueTask<T>>> ThrowsAsync<T>(
        this IReturnValueConfiguration<ValueTask<T>> configuration,
        Func<Exception> exceptionFactory)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(exceptionFactory);

        return configuration.ReturnsLazily(call => new ValueTask<T>(TaskHelper.FromException<T>(exceptionFactory())));
    }
}
