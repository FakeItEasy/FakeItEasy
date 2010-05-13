namespace FakeItEasy.DynamicProxy
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// An interface implemented by DynamicProxy proxies in order to let them
    /// intercept object members.
    /// 
    /// NOT INTENDED FOR USE! Note that this interface WILL be removed from future versions of FakeItEasy.
    /// </summary>
    public interface ICanInterceptObjectMembers
    {
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        string ToString();

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="o">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "o", Justification = "Uses the same name as the hidden method.")]
        bool Equals(object o);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        int GetHashCode();
    }
}
