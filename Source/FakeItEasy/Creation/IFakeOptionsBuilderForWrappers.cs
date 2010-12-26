namespace FakeItEasy.Creation
{
    using FakeItEasy.SelfInitializedFakes;

    /// <summary>
    /// Provides options for fake wrappers.
    /// </summary>
    /// <typeparam name="T">The type of the fake object generated.</typeparam>
    public interface IFakeOptionsBuilderForWrappers<T>
        : IFakeOptionsBuilder<T>
    {
        /// <summary>
        /// Specifies a fake recorder to use.
        /// </summary>
        /// <param name="recorder">The recorder to use.</param>
        /// <returns>Options object.</returns>
        IFakeOptionsBuilder<T> RecordedBy(ISelfInitializingFakeRecorder recorder);
    }
}