namespace FakeItEasy.Creation
{
    using FakeItEasy.SelfInitializedFakes;

    /// <summary>
    /// Provides options for fake wrappers.
    /// </summary>
    public interface IFakeOptionsForWrappers
        : IFakeOptions
    {
        /// <summary>
        /// Specifies a fake recorder to use.
        /// </summary>
        /// <param name="recorder">The recorder to use.</param>
        /// <returns>Options object.</returns>
        IFakeOptions RecordedBy(ISelfInitializingFakeRecorder recorder);
    }
}
