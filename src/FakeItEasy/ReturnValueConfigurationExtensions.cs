namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using System.Threading.Tasks;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for <see cref="IReturnValueConfiguration{T}"/>.
    /// </summary>
    public static class ReturnValueConfigurationExtensions
    {
        private const string NameOfReturnsLazilyFeature = "returns lazily";

        /// <summary>
        /// Specifies the value to return when the configured call is made.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="value">The value to return.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<T>> Returns<T>(this IReturnValueConfiguration<T> configuration, T value)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.ReturnsLazily(x => value);
        }

        /// <summary>
        /// Specifies the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/> which is returned when the configured call is made.
        /// The <see cref="Task{T}"/> returned from the configured call will have a <see cref="Task.Status"/> of <see cref="TaskStatus.RanToCompletion"/>.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the <see cref="Task{T}"/>.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="value">The <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/>.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of Task<T> return values.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<T>>> Returns<T>(this IReturnValueConfiguration<Task<T>> configuration, T value)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.ReturnsLazily(() => value);
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<T>> ReturnsLazily<T>(this IReturnValueConfiguration<T> configuration, Func<T> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(valueProducer, nameof(valueProducer));

            return configuration.ReturnsLazily(x => valueProducer());
        }

        /// <summary>
        /// Specifies a function used to produce the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/> which is returned when the configured call is made.
        /// The function will be called each time the configured call is made and can return different values each time.
        /// The <see cref="Task{T}"/> returned from the configured call will have a <see cref="Task.Status"/> of <see cref="TaskStatus.RanToCompletion"/>.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the <see cref="Task{T}"/>.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/>.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of Task<T> return values.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<T>>> ReturnsLazily<T>(this IReturnValueConfiguration<Task<T>> configuration, Func<T> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(valueProducer, nameof(valueProducer));

            return configuration.ReturnsLazily(x => TaskHelper.FromResult(valueProducer()));
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the return value.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<TReturnType>>
            ReturnsLazily<TReturnType, T1>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0));
                });
        }

        /// <summary>
        /// Specifies a function used to produce the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/> which is returned when the configured call is made.
        /// The function will be called each time the configured call is made and can return different values each time.
        /// The <see cref="Task{T}"/> returned from the configured call will have a <see cref="Task.Status"/> of <see cref="TaskStatus.RanToCompletion"/>.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the result produced by the <see cref="Task{T}"/>.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/>.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of Task<T> return values.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<TReturnType>>>
            ReturnsLazily<TReturnType, T1>(this IReturnValueConfiguration<Task<TReturnType>> configuration, Func<T1, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(valueProducer, nameof(valueProducer));

            return configuration.ReturnsLazily(call =>
            {
                ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                return TaskHelper.FromResult(valueProducer(call.GetArgument<T1>(0)));
            });
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <typeparam name="TReturnType">The type of the return value.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<TReturnType>>
            ReturnsLazily<TReturnType, T1, T2>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, T2, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
                });
        }

        /// <summary>
        /// Specifies a function used to produce the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/> which is returned when the configured call is made.
        /// The function will be called each time the configured call is made and can return different values each time.
        /// The <see cref="Task{T}"/> returned from the configured call will have a <see cref="Task.Status"/> of <see cref="TaskStatus.RanToCompletion"/>.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the result produced by the <see cref="Task{T}"/>.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/>.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of Task<T> return values.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<TReturnType>>>
            ReturnsLazily<TReturnType, T1, T2>(this IReturnValueConfiguration<Task<TReturnType>> configuration, Func<T1, T2, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.ReturnsLazily(call =>
            {
                ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                return TaskHelper.FromResult(valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1)));
            });
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <typeparam name="TReturnType">The type of the return value.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<TReturnType>>
            ReturnsLazily<TReturnType, T1, T2, T3>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, T2, T3, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
                });
        }

        /// <summary>
        /// Specifies a function used to produce the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/> which is returned when the configured call is made.
        /// The function will be called each time the configured call is made and can return different values each time.
        /// The <see cref="Task{T}"/> returned from the configured call will have a <see cref="Task.Status"/> of <see cref="TaskStatus.RanToCompletion"/>.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the result produced by the <see cref="Task{T}"/>.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/>.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of Task<T> return values.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<TReturnType>>>
            ReturnsLazily<TReturnType, T1, T2, T3>(this IReturnValueConfiguration<Task<TReturnType>> configuration, Func<T1, T2, T3, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                    return TaskHelper.FromResult(valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2)));
                });
        }

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <typeparam name="TReturnType">The type of the return value.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design to support the fluent API.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<TReturnType>>
            ReturnsLazily<TReturnType, T1, T2, T3, T4>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, T2, T3, T4, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
                });
        }

        /// <summary>
        /// Specifies a function used to produce the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/> which is returned when the configured call is made.
        /// The function will be called each time the configured call is made and can return different values each time.
        /// The <see cref="Task{T}"/> returned from the configured call will have a <see cref="Task.Status"/> of <see cref="TaskStatus.RanToCompletion"/>.
        /// </summary>
        /// <typeparam name="TReturnType">The type of the result produced by the <see cref="Task{T}"/>.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/>.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of Task<T> return values.")]
        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<TReturnType>>>
            ReturnsLazily<TReturnType, T1, T2, T3, T4>(this IReturnValueConfiguration<Task<TReturnType>> configuration, Func<T1, T2, T3, T4, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                    return TaskHelper.FromResult(valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3)));
                });
        }

        /// <summary>
        /// Configures the call to return the next value from the specified sequence each time it's called.
        /// After the sequence has been exhausted, the call will revert to the previously configured behavior.
        /// </summary>
        /// <typeparam name="T">The type of return value.</typeparam>
        /// <param name="configuration">The call configuration to extend.</param>
        /// <param name="values">The values to return in sequence.</param>
        public static void ReturnsNextFromSequence<T>(this IReturnValueConfiguration<T> configuration, params T[] values)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            var queue = new Queue<T>(values);
            configuration.ReturnsLazily(x => queue.Dequeue()).NumberOfTimes(queue.Count);
        }

        /// <summary>
        /// Configures the call to return a <see cref="Task{T}"/> with a <see cref="Task{T}.Result"/> of
        /// the next value from the specified sequence each time it's called.
        /// After the sequence has been exhausted, the call will revert to the previously configured behavior.
        /// Each <see cref="Task{T}"/> returned from the configured call will have a <see cref="Task.Status"/> of <see cref="TaskStatus.RanToCompletion"/>.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by each <see cref="Task{T}"/>.</typeparam>
        /// <param name="configuration">The call configuration to extend.</param>
        /// <param name="values">The values to use for the <see cref="Task{T}.Result"/> of each <see cref="Task{T}"/> in sequence.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of Task<T> return values.")]
        public static void ReturnsNextFromSequence<T>(this IReturnValueConfiguration<Task<T>> configuration, params T[] values)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            var taskValues = values.Select(value => TaskHelper.FromResult(value));

            var queue = new Queue<Task<T>>(taskValues);

            configuration.ReturnsLazily(x => queue.Dequeue()).NumberOfTimes(queue.Count);
        }
    }
}
