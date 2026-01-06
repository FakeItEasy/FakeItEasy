namespace FakeItEasy;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy.Configuration;

/// <summary>
/// Provides extension methods for <see cref="IReturnValueConfiguration{T}"/> to configure methods returning <see cref="ValueTask{TResult}"/>.
/// </summary>
public static partial class ValueTaskReturnValueConfigurationExtensions
{
    /// <summary>
    /// Specifies the <see cref="ValueTask{T}.Result"/> of the <see cref="ValueTask{T}"/> which is returned when the configured call is made.
    /// The <see cref="ValueTask{T}"/> returned from the configured call will have its <see cref="ValueTask{T}.IsCompletedSuccessfully"/> property set to <c>true</c>".
    /// </summary>
    /// <typeparam name="T">The type of the result produced by the <see cref="ValueTask{T}"/>.</typeparam>
    /// <param name="configuration">The configuration to extend.</param>
    /// <param name="value">The <see cref="ValueTask{T}.Result"/> of the <see cref="ValueTask{T}"/>.</param>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of ValueTask<T> return values.")]
    public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<ValueTask<T>>> Returns<T>(this IReturnValueConfiguration<ValueTask<T>> configuration, T value)
    {
        Guard.AgainstNull(configuration);

        return configuration.ReturnsLazily(() => value);
    }

    /// <summary>
    /// Specifies a function used to produce the <see cref="ValueTask{T}.Result"/> of the <see cref="ValueTask{T}"/> which is returned when the configured call is made.
    /// The function will be called each time the configured call is made and can return different values each time.
    /// The <see cref="ValueTask{T}"/> returned from the configured call will have its <see cref="ValueTask{T}.IsCompletedSuccessfully"/> property set to <c>true</c>".
    /// </summary>
    /// <typeparam name="T">The type of the result produced by the <see cref="ValueTask{T}"/>.</typeparam>
    /// <param name="configuration">The configuration to extend.</param>
    /// <param name="valueProducer">A function that produces the <see cref="ValueTask{T}.Result"/> of the <see cref="ValueTask{T}"/>.</param>
    /// <returns>The configuration object.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of ValueTask<T> return values.")]
    public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<ValueTask<T>>> ReturnsLazily<T>(this IReturnValueConfiguration<ValueTask<T>> configuration, Func<T> valueProducer)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(valueProducer);

        return configuration.ReturnsLazily(x => new ValueTask<T>(valueProducer()));
    }

    /// <summary>
    /// Configures the call to return a <see cref="ValueTask{T}"/> with a <see cref="ValueTask{T}.Result"/> of
    /// the next value from the specified sequence each time it's called.
    /// After the sequence has been exhausted, the call will revert to the previously configured behavior.
    /// Each <see cref="ValueTask{T}"/> returned from the configured call will have its <see cref="ValueTask{T}.IsCompletedSuccessfully"/> property set to <c>true</c>".
    /// </summary>
    /// <typeparam name="T">The type of the result produced by each <see cref="ValueTask{T}"/>.</typeparam>
    /// <param name="configuration">The call configuration to extend.</param>
    /// <param name="values">The values to use for the <see cref="ValueTask{T}.Result"/> of each <see cref="ValueTask{T}"/> in sequence.</param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of ValueTask<T> return values.")]
    public static void ReturnsNextFromSequence<T>(this IReturnValueConfiguration<ValueTask<T>> configuration, params T[] values)
    {
        Guard.AgainstNull(configuration);
        Guard.AgainstNull(values);

        configuration.ReturnsNextFromQueue(new ConcurrentQueue<ValueTask<T>>(values.Select(value => new ValueTask<T>(value))));
    }
}
