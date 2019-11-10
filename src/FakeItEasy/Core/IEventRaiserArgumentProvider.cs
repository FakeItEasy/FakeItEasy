namespace FakeItEasy.Core
{
    /// <summary>
    /// Used by the event raising rule of fake objects to get the event arguments used in
    /// a call to Raise.With.
    /// </summary>
    internal interface IEventRaiserArgumentProvider
    {
        /// <summary>
        /// Gets the event arguments of the event.
        /// </summary>
        /// <param name="fake">The fake that is raising the event.</param>
        /// <returns>The event arguments.</returns>
        object?[] GetEventArguments(object fake);
    }
}
