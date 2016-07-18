namespace FakeItEasy.Creation
{
#if FEATURE_SELF_INITIALIZED_FAKES
    using FakeItEasy.SelfInitializedFakes;
#endif

    /// <summary>
    /// Provides options for fake wrappers.
    /// </summary>
    /// <typeparam name="T">The type of the fake object generated.</typeparam>
    public interface IFakeOptionsForWrappers<T>
        : IFakeOptions<T>
    {
#if FEATURE_SELF_INITIALIZED_FAKES
        /// <summary>
        /// Specifies a fake recorder to use.
        /// </summary>
        /// <param name="recorder">The recorder to use.</param>
        /// <returns>Options object.</returns>
        IFakeOptions<T> RecordedBy(ISelfInitializingFakeRecorder recorder);
#endif
    }
}
