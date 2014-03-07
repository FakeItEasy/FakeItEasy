namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
#if NET40
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
#endif

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
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns<T>(this IReturnValueConfiguration<T> configuration, T value)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(x => value);
        }

#if NET40
        /// <summary>
        /// Specifies the value to return when the configured call is made.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="value">The value to return.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary to support special handling of Task<T> return values.")]
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration Returns<T>(this IReturnValueConfiguration<Task<T>> configuration, T value)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(() => value);
        }
#endif

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration ReturnsLazily<T>(this IReturnValueConfiguration<T> configuration, Func<T> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(valueProducer, "valueProducer");

            return configuration.ReturnsLazily(x => valueProducer());
        }

#if NET40
        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration ReturnsLazily<T>(this IReturnValueConfiguration<Task<T>> configuration, Func<T> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(valueProducer, "valueProducer");

            var taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetResult(valueProducer());

            return configuration.ReturnsLazily(x => taskCompletionSource.Task);
        }
#endif

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
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer" /> do not match.</exception>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0));
                });
        }

#if NET40
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
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer" /> do not match.</exception>
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1>(this IReturnValueConfiguration<Task<TReturnType>> configuration, Func<T1, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(valueProducer, "valueProducer");

            return configuration.ReturnsLazily(call =>
            {
                var taskCompletionSource = new TaskCompletionSource<TReturnType>();
                taskCompletionSource.SetResult(valueProducer(call.GetArgument<T1>(0)));

                return taskCompletionSource.Task;
            });
        }
#endif

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
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1, T2>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, T2, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
                });
        }

#if NET40
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
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1, T2>(this IReturnValueConfiguration<Task<TReturnType>> configuration, Func<T1, T2, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
            {
                ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                var taskCompletionSource = new TaskCompletionSource<TReturnType>();
                taskCompletionSource.SetResult(valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1)));
                return taskCompletionSource.Task;
            });
        }
#endif

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
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1, T2, T3>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, T2, T3, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
                });
        }

#if NET40
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
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1, T2, T3>(this IReturnValueConfiguration<Task<TReturnType>> configuration, Func<T1, T2, T3, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    var taskCompletionSource = new TaskCompletionSource<TReturnType>();
                    taskCompletionSource.SetResult(valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2)));
                    return taskCompletionSource.Task;
                });
        }
#endif

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
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1, T2, T3, T4>(this IReturnValueConfiguration<TReturnType> configuration, Func<T1, T2, T3, T4, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
                });
        }

#if NET40
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
        public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration
            ReturnsLazily<TReturnType, T1, T2, T3, T4>(this IReturnValueConfiguration<Task<TReturnType>> configuration, Func<T1, T2, T3, T4, TReturnType> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.Method, NameOfReturnsLazilyFeature);

                    var taskCompletionSource = new TaskCompletionSource<TReturnType>();
                    taskCompletionSource.SetResult(valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3)));
                    return taskCompletionSource.Task;
                });
        }
#endif

        /// <summary>
        /// Configures the call to return the next value from the specified sequence each time it's called. Null will
        /// be returned when all the values in the sequence has been returned.
        /// </summary>
        /// <typeparam name="T">
        /// The type of return value.
        /// </typeparam>
        /// <param name="configuration">
        /// The call configuration to extend.
        /// </param>
        /// <param name="values">
        /// The values to return in sequence.
        /// </param>
        public static void ReturnsNextFromSequence<T>(this IReturnValueConfiguration<T> configuration, params T[] values)
        {
            Guard.AgainstNull(configuration, "configuration");

            var queue = new Queue<T>(values);
            configuration.ReturnsLazily(x => queue.Dequeue()).NumberOfTimes(queue.Count);
        }

#if NET40
        /// <summary>
        /// Configures the call to return the next value from the specified sequence each time it's called. Null will
        /// be returned when all the values in the sequence has been returned.
        /// </summary>
        /// <typeparam name="T">
        /// The type of return value.
        /// </typeparam>
        /// <param name="configuration">
        /// The call configuration to extend.
        /// </param>
        /// <param name="values">
        /// The values to return in sequence.
        /// </param>
        public static void ReturnsNextFromSequence<T>(this IReturnValueConfiguration<Task<T>> configuration, params T[] values)
        {
            Guard.AgainstNull(configuration, "configuration");

            var taskValues = values.Select(value =>
            {
                var tsc = new TaskCompletionSource<T>();
                tsc.SetResult(value);
                return tsc.Task;
            });

            var queue = new Queue<Task<T>>(taskValues);

            configuration.ReturnsLazily(x => queue.Dequeue()).NumberOfTimes(queue.Count);
        }
#endif
    }
}