namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides the extension methods for <see cref="IExceptionThrowerConfiguration"/>.
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
        /// <returns>Configuration object.</returns>
        public static IAfterCallSpecifiedConfiguration Throws(this IExceptionThrowerConfiguration configuration, Exception exception)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(_ => exception);
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <returns>Configuration object.</returns>
        public static IAfterCallSpecifiedConfiguration Throws(this IExceptionThrowerConfiguration configuration, Func<Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(_ => exceptionFactory());
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        public static IAfterCallSpecifiedConfiguration Throws<T1>(this IExceptionThrowerConfiguration configuration, Func<T1, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, exceptionFactory.Method, NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        public static IAfterCallSpecifiedConfiguration Throws<T1, T2>(this IExceptionThrowerConfiguration configuration, Func<T1, T2, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, exceptionFactory.Method, NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        public static IAfterCallSpecifiedConfiguration Throws<T1, T2, T3>(this IExceptionThrowerConfiguration configuration, Func<T1, T2, T3, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, exceptionFactory.Method, NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <returns>Configuration object.</returns>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
        public static IAfterCallSpecifiedConfiguration Throws<T1, T2, T3, T4>(this IExceptionThrowerConfiguration configuration, Func<T1, T2, T3, T4, Exception> exceptionFactory)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, exceptionFactory.Method, NameOfThrowsFeature);

                    return exceptionFactory(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
                });
        }

        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <typeparam name="T">The type of exception to throw.</typeparam>
        /// <returns>Configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By design.")]
        public static IAfterCallSpecifiedConfiguration Throws<T>(this IExceptionThrowerConfiguration configuration) where T : Exception, new()
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.Throws(_ => new T());
        }
    }
}