namespace FakeItEasy
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Configuration;

    /// <summary>
    /// Provides configuration for any (not a specific) call on a faked object.
    /// </summary>
    [Obsolete("Methods provided by this class are now available directly on the A-class.")]
    public static class Any
    {
        /// <summary>
        /// Gets a configuration object allowing for further configuration of
        /// any calll to the specified faked object.
        /// </summary>
        /// <param name="fakedObject">The faked object to configure.</param>
        /// <returns>A configuration object.</returns>
        public static IAnyCallConfiguration CallTo(object fakedObject)
        {
            return A.CallTo(fakedObject);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>True if the two objects are equal.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Using the same names as the hidden method.")]
        public static new bool Equals(object objA, object objB)
        {
            return object.Equals(objA, objB);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are the same reference.
        /// </summary>
        /// <param name="objA">The obj A.</param>
        /// <param name="objB">The obj B.</param>
        /// <returns>True if the objects are the same reference.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Using the same names as the hidden method.")]
        public static new bool ReferenceEquals(object objA, object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }
    }
}