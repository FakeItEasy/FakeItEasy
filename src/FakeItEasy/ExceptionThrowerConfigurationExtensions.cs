namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides the extension methods for <see cref="IExceptionThrowerConfiguration{TInterface}"/>.
    /// </summary>
    public static class ExceptionThrowerConfigurationExtensions
    {
        private const string NameOfThrowsFeature = "throws";

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exception">The exception to throw when a call that matches is invoked.</param>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <returns>Configuration object.</returns>
        public static IAfterCallConfiguredConfiguration<TInterface> Throws<TInterface>(this IExceptionThrowerConfiguration<TInterface> configuration, Exception exception)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Throws(_ => exception);
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <returns>Configuration object.</returns>
        public static IAfterCallConfiguredConfiguration<TInterface> Throws<TInterface>(this IExceptionThrowerConfiguration<TInterface> configuration, Func<Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Throws(_ => exceptionFactory());
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        internal static IAfterCallConfiguredConfiguration<TInterface> Throws<TInterface, T1>(this IExceptionThrowerConfiguration<TInterface> configuration, Func<T1, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Throws(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, exceptionFactory.GetMethodInfo(), NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        internal static IAfterCallConfiguredConfiguration<TInterface> Throws<TInterface, T1, T2>(this IExceptionThrowerConfiguration<TInterface> configuration, Func<T1, T2, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Throws(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, exceptionFactory.GetMethodInfo(), NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        internal static IAfterCallConfiguredConfiguration<TInterface> Throws<TInterface, T1, T2, T3>(this IExceptionThrowerConfiguration<TInterface> configuration, Func<T1, T2, T3, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Throws(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, exceptionFactory.GetMethodInfo(), NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        internal static IAfterCallConfiguredConfiguration<TInterface> Throws<TInterface, T1, T2, T3, T4>(this IExceptionThrowerConfiguration<TInterface> configuration, Func<T1, T2, T3, T4, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Throws(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, exceptionFactory.GetMethodInfo(), NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <typeparam name="T">The type of exception to throw.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
        internal static IAfterCallConfiguredConfiguration<TInterface> Throws<TInterface, T>(this IExceptionThrowerConfiguration<TInterface> configuration) where T : Exception, new()
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Throws(_ => new T());
        }
    }
}
