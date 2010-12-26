namespace FakeItEasy.ExtensionSyntax
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides an extension method for configuring fake objects.
    /// </summary>
    public static class Syntax
    {
        /// <summary>
        /// Gets an object that provides a fluent interface syntax for configuring
        /// the fake object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake object.</typeparam>
        /// <param name="fakedObject">The fake object to configure.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="System.ArgumentNullException">The fakedObject was null.</exception>
        /// <exception cref="System.ArgumentException">The object passed in is not a faked object.</exception>
        public static IStartConfiguration<TFake> Configure<TFake>(this TFake fakedObject)
        {
            var factory = ServiceLocator.Current.Resolve<IStartConfigurationFactory>();
            return factory.CreateConfiguration<TFake>(Fake.GetFakeManager(fakedObject));
        }
    }
}