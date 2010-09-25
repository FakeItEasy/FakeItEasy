namespace FakeItEasy.Core
{
    using FakeItEasy.SelfInitializedFakes;

    /// <summary>
    /// Manages configuration of fake objects to wrap instances.
    /// </summary>
    internal interface IFakeWrapperConfigurer
    {
        /// <summary>
        /// Configures the specified faked object to wrap the specified instance.
        /// </summary>
        /// <param name="fakedObject">The faked object to configure.</param>
        /// <param name="wrappedInstance">The instance to wrap.</param>
        /// <param name="recorder">The recorder to use, null if no recording should be made.</param>
        void ConfigureFakeToWrap(object fakedObject, object wrappedInstance, ISelfInitializingFakeRecorder recorder);
    }
}
