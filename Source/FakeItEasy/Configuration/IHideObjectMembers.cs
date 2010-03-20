namespace FakeItEasy.Configuration
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Hides standard Object members to make fluent interfaces
    /// easier to read. Found in the source of Autofac: http://code.google.com/p/autofac/
    /// Based on blog post by @kzu here:
    /// http://www.clariusconsulting.net/blogs/kzu/archive/2008/03/10/58301.aspx
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IHideObjectMembers
    {
        /// <summary>
        /// Hides the ToString-method.
        /// </summary>
        /// <returns>A string representation of the implementing object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="o">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object o);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();
    }
}
