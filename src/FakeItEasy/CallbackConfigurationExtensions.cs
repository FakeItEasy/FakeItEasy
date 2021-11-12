namespace FakeItEasy
{
    using System;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for <see cref="ICallbackConfiguration{TFake}"/>.
    /// </summary>
    public static partial class CallbackConfigurationExtensions
    {
        private const string NameOfInvokesFeature = "invokes";

        /// <summary>
        /// Executes the specified action when a matching call is being made. This overload can also be used to fake calls with arguments when they don't need to be accessed.
        /// </summary>
        /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
        /// <param name="configuration">The configuration that is extended.</param>
        /// <param name="actionToInvoke">The <see cref="Action" /> to invoke.</param>
        /// <returns>The fake object.</returns>
        public static TInterface Invokes<TInterface>(this ICallbackConfiguration<TInterface> configuration, Action actionToInvoke)
        {
            Guard.AgainstNull(configuration);
            Guard.AgainstNull(actionToInvoke);

            return configuration.Invokes(call => actionToInvoke());
        }
    }
}
