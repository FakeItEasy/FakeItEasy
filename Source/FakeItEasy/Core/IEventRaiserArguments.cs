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
        /// The sender of the event.
        /// </summary>
        object Sender { get; }

        /// <summary>
        /// The event arguments of the event.
        /// </summary>
        EventArgs EventArguments { get; }
    }
}