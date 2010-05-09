namespace FakeItEasy.Configuration
{
    using FakeItEasy.Core;

    /// <summary>
    /// A factory responsible for creating start configuration for fake objects.
    /// </summary>
    internal interface IStartConfigurationFactory
    {
        /// <summary>
        /// Creates a start configuration for the specified fake object that fakes the
        /// specified type.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake object.</typeparam>
        /// <param name="fakeObject">The fake object to configure.</param>
        /// <returns>A configuration object.</returns>
        IStartConfiguration<TFake> CreateConfiguration<TFake>(FakeManager fakeObject);
    }
}
