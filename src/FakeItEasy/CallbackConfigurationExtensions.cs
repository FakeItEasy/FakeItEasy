namespace FakeItEasy
{
    using System;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for <see cref="ICallbackConfiguration{TFake}"/>.
    /// </summary>
    public static class CallbackConfigurationExtensions
    {
        private const string NameOfInvokesFeature = "invokes";

        /// <summary>
        /// Executes the specified action when a matching call is being made. This overload can also be used to fake calls with arguments when they don't need to be accessed.
        /// </summary>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action" /> to invoke.</param>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake>(this ICallbackConfiguration<TFake> configuration, Action actionToInvoke)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Invokes(call => actionToInvoke());
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1}"/> to invoke.</param>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="actionToInvoke"/> do not match.</exception>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake, T1>(this ICallbackConfiguration<TFake> configuration, Action<T1> actionToInvoke)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Invokes(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, actionToInvoke.GetMethodInfo(), NameOfInvokesFeature);

                    actionToInvoke(call.GetArgument<T1>(0));
                });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1,T2}"/> to invoke.</param>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="actionToInvoke"/> do not match.</exception>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake, T1, T2>(this ICallbackConfiguration<TFake> configuration, Action<T1, T2> actionToInvoke)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Invokes(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, actionToInvoke.GetMethodInfo(), NameOfInvokesFeature);

                    actionToInvoke(call.GetArgument<T1>(0), call.GetArgument<T2>(1));
                });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1,T2,T3}"/> to invoke.</param>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="actionToInvoke"/> do not match.</exception>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake, T1, T2, T3>(this ICallbackConfiguration<TFake> configuration, Action<T1, T2, T3> actionToInvoke)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Invokes(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, actionToInvoke.GetMethodInfo(), NameOfInvokesFeature);

                    actionToInvoke(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2));
                });
        }

        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action{T1,T2,T3,T4}"/> to invoke.</param>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <typeparam name="T1">Type of the first argument of the faked method call.</typeparam>
        /// <typeparam name="T2">Type of the second argument of the faked method call.</typeparam>
        /// <typeparam name="T3">Type of the third argument of the faked method call.</typeparam>
        /// <typeparam name="T4">Type of the fourth argument of the faked method call.</typeparam>
        /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="actionToInvoke"/> do not match.</exception>
        /// <returns>The fake object.</returns>
        public static TFake Invokes<TFake, T1, T2, T3, T4>(this ICallbackConfiguration<TFake> configuration, Action<T1, T2, T3, T4> actionToInvoke)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            return configuration.Invokes(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, actionToInvoke.GetMethodInfo(), NameOfInvokesFeature);

                    actionToInvoke(call.GetArgument<T1>(0), call.GetArgument<T2>(1), call.GetArgument<T3>(2), call.GetArgument<T4>(3));
                });
        }
    }
}
