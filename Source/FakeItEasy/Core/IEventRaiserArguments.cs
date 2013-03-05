namespace FakeItEasy.Core
{
    using System;

    /// <summary>
    /// Used by the event raising rule of fake objects to get the event arguments used in
    /// a call to Raise.With.
    /// </summary>
    internal interface IEventRaiserArguments
    {
        /// <summary>
        /// Gets the sender of the event.
        /// </summary>
        object Sender { get; }

        /// <summary>
        /// Gets the event arguments of the event.
        /// </summary>
        EventArgs EventArguments { get; }
    }
}