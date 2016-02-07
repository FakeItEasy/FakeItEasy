namespace FakeItEasy.Core
{
    /// <summary>
    /// Allows for intercepting call to a fake object and
    /// act upon them.
    /// </summary>
    public interface IFakeObjectCallRule
    {
        /// <summary>
        /// Gets the number of times this call rule is valid, if it's set
        /// to null its infinitely valid.
        /// </summary>
        int? NumberOfTimesToCall { get; }

        /// <summary>
        /// Gets whether this interceptor is applicable to the specified
        /// call, if true is returned the Apply-method of the interceptor will
        /// be called.
        /// </summary>
        /// <param name="fakeObjectCall">The call to check for applicability.</param>
        /// <returns>True if the interceptor is applicable.</returns>
        bool IsApplicableTo(IFakeObjectCall fakeObjectCall);

        /// <summary>
        /// Applies an action to the call, might set a return value or throw
        /// an exception.
        /// </summary>
        /// <param name="fakeObjectCall">The call to apply the interceptor to.</param>
        void Apply(IInterceptedFakeObjectCall fakeObjectCall);
    }
}
