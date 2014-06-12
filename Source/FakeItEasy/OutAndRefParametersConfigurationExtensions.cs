namespace FakeItEasy
{
    using System;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for <see cref="IOutAndRefParametersConfiguration"/>.
    /// </summary>
    public static class OutAndRefParametersConfigurationExtensions
    {
        private const string NameOfOutRefLazilyFeature = "assigns out and ref parameters lazily";

        /// <summary>
        /// Specifies output values for out and ref parameters. The values should appear 
        /// in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="values">The values.</param>
        /// <returns>A configuration object.</returns>
        public static IAfterCallSpecifiedConfiguration AssignsOutAndRefParameters(
            this IOutAndRefParametersConfiguration configuration, params object[] values)
        {
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(values, "values");

            return configuration.AssignsOutAndRefParametersLazily(x => values);
        }

        /// <summary>
        /// Specifies a function used to produce output values for out and ref parameters.
        /// The values should appear in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">
        /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
        /// </exception>
        public static IAfterCallSpecifiedConfiguration AssignsOutAndRefParametersLazily<T1>(
            this IOutAndRefParametersConfiguration configuration, Func<T1, object[]> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.AssignsOutAndRefParametersLazily(call =>
            {
                ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(
                    call.Method, valueProducer.Method, NameOfOutRefLazilyFeature);

                return valueProducer(call.GetArgument<T1>(0));
            });
        }

        /// <summary>
        /// Specifies a function used to produce output values for out and ref parameters.
        /// The values should appear in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">
        /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
        /// </exception>
        public static IAfterCallSpecifiedConfiguration
            AssignsOutAndRefParametersLazily<T1, T2>(
            this IOutAndRefParametersConfiguration configuration, Func<T1, T2, object[]> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.AssignsOutAndRefParametersLazily(call =>
            {
                ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(
                    call.Method, valueProducer.Method, NameOfOutRefLazilyFeature);

                return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
            });
        }

        /// <summary>
        /// Specifies a function used to produce output values for out and ref parameters.
        /// The values should appear in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">
        /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
        /// </exception>
        public static IAfterCallSpecifiedConfiguration
            AssignsOutAndRefParametersLazily<T1, T2, T3>(
            this IOutAndRefParametersConfiguration configuration, Func<T1, T2, T3, object[]> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.AssignsOutAndRefParametersLazily(call =>
            {
                ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(
                    call.Method, valueProducer.Method, NameOfOutRefLazilyFeature);

                return valueProducer(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
            });
        }

        /// <summary>
        /// Specifies a function used to produce output values for out and ref parameters.
        /// The values should appear in the same order as the out and ref parameters in the configured call.
        /// Any non out and ref parameters are ignored.
        /// The function will be called each time this call is made and can return different values.
        /// </summary>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <param name="configuration">The configuration to extend.</param>
        /// <param name="valueProducer">A function that produces the output values.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="FakeConfigurationException">
        /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
        /// </exception>
        public static IAfterCallSpecifiedConfiguration AssignsOutAndRefParametersLazily<T1, T2, T3, T4>(
            this IOutAndRefParametersConfiguration configuration, Func<T1, T2, T3, T4, object[]> valueProducer)
        {
            Guard.AgainstNull(configuration, "configuration");

            return configuration.AssignsOutAndRefParametersLazily(call =>
            {
                ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(
                    call.Method, valueProducer.Method, NameOfOutRefLazilyFeature);

                return valueProducer(
                    call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
            });
        }
    }
}