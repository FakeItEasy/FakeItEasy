namespace FakeItEasy.Creation
{
#if FEATURE_SELF_INITIALIZED_FAKES
    using FakeItEasy.SelfInitializedFakes;
#endif

    /// <summary>
    /// Provides options for fake wrappers.
    /// </summary>
    public interface IFakeOptionsForWrappers
        : IFakeOptions
    {
#if FEATURE_SELF_INITIALIZED_FAKES
        /// <summary>
        /// Specifies a fake recorder to use.
        /// </summary>
        /// <param name="recorder">The recorder to use.</param>
        /// <returns>Options object.</returns>
        IFakeOptions RecordedBy(ISelfInitializingFakeRecorder recorder);
#endif
    }
}
