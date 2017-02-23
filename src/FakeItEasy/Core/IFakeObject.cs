namespace FakeItEasy.Core
{
    /// <summary>
    /// This interface is used internally by FakeItEasy and is exposed only due to technical constraints. It is not
    /// intended to be used directly from your code.
    /// </summary>
    public interface IFakeObject
    {
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        bool Equals(object other);

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        int GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        string ToString();
    }
}
