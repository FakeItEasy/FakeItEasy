namespace FakeItEasy.Creation
{
    using System;
    using FakeItEasy.SelfInitializedFakes;

    /// <summary>
    /// Provides options for fake wrappers.
    /// </summary>
    /// <typeparam name="T">The type of the fake object generated.</typeparam>
    public interface IFakeOptionsForWrappers<T>
        : IFakeOptions<T>
    {
        /// <summary>
        /// Specifies a fake recorder to use.
        /// </summary>
        /// <param name="recorder">The recorder to use.</param>
        /// <returns>Options object.</returns>
        [Obsolete("Self-initializing fakes will be removed in version 4.0.0.")]
        IFakeOptions<T> RecordedBy(ISelfInitializingFakeRecorder recorder);
    }
}
