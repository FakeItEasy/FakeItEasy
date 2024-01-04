namespace FakeItEasy
{
    using System;

    /// <summary>
    /// Holds argument values captured from calls to a faked member.
    /// </summary>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    public class Captured<TArgument> : Captured<TArgument, TArgument>
    {
        private static readonly Func<TArgument, TArgument> Identity = t => t;

        /// <summary>
        /// Initializes a new instance of the <see cref="Captured{TArgument}"/> class.
        /// </summary>
        internal Captured()
            : base(Identity)
        {
        }

        /// <summary>
        /// Intiializes a new instance of the <see cref="Captured{TArgument, TCapture}" /> class.
        /// </summary>
        /// <remarks>
        /// The new instance will, when capturing arguments, "freeze" them by applying
        /// the <paramref name="freezer"/> function so they can be insulated from changes
        /// to the original argument values after the call is made.
        /// </remarks>
        /// <typeparam name="TCapture">The type of the transformed argument value to capture.</typeparam>
        /// <param name="freezer">
        /// Transforms incoming argument values before storing the result.
        /// Useful when argument values may be mutated between calls and you want
        /// to store a copy that will not be modified.
        /// </param>
        /// <returns>
        /// A new <c>Captured</c> instance that can be used to capture arguments
        /// to fake calls.
        /// </returns>
        public Captured<TArgument, TCapture> FrozenBy<TCapture>(Func<TArgument, TCapture> freezer)
            => new(freezer);
    }
}
