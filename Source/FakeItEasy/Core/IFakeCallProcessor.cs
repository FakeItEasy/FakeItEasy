namespace FakeItEasy.Core
{
    /// <summary>
    /// Represents the target of intercepted calls of a fake proxy. An implementation of this interface receives calls, gets its arguments
    /// and can provide return values.
    /// </summary>
    internal interface IFakeCallProcessor
    {
        /// <summary>
        /// Processes an intercepted call of a fake proxy.
        /// </summary>
        /// <param name="fakeObjectCall">The call information (like which method has been called, its arguments, ...).</param>
        void Process(IWritableFakeObjectCall fakeObjectCall);
    }
}