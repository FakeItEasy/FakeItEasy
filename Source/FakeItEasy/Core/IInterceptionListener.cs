namespace FakeItEasy.Core
{
    /// <summary>
    /// Represents a listener for fake object calls, can be plugged into a
    /// FakeManager instance to listen to all intercepted calls.
    /// </summary>
    /// <remarks>The OnBeforeCallIntercepted method will be invoked before the OnBeforeCallIntercepted method of any
    /// previously added listener. The OnAfterCallIntercepted method will be invoked after the OnAfterCallIntercepted
    /// method of any previously added listener.</remarks>
    public interface IInterceptionListener
    {
        /// <summary>
        /// Called when the interception begins but before any call rules
        /// has been applied.
        /// </summary>
        /// <param name="call">The intercepted call.</param>
        void OnBeforeCallIntercepted(IFakeObjectCall call);

        /// <summary>
        /// Called when the interception has been completed and rules has been
        /// applied.
        /// </summary>
        /// <param name="ruleThatWasApplied">The rule that was applied to the call.</param>
        /// <param name="call">The intercepted call.</param>
        void OnAfterCallIntercepted(ICompletedFakeObjectCall call, IFakeObjectCallRule ruleThatWasApplied);
    }
}