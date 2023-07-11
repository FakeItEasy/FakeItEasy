namespace FakeItEasy
{
    using System;

    /// <summary>
    /// Provides custom equality comparison for call arguments.
    /// </summary>
    public interface IArgumentEqualityComparer
    {
        /// <summary>
        /// Gets the priority of the argument equality comparer. When multiple comparers that apply to the same type are registered,
        /// the one with the highest priority value is used.
        /// </summary>
        Priority Priority { get; }

        /// <summary>
        /// Whether or not this object can compare instances of <paramref name="type"/> for equality.
        /// </summary>
        /// <param name="type">The type of the objects to compare.</param>
        /// <returns>
        /// <c>true</c> if this object can compare instances of <paramref name="type"/>. Otherwise <c>false</c>.
        /// </returns>
        bool CanCompare(Type type);

        /// <summary>
        /// Indicates whether <paramref name="expectedValue"/> and <paramref name="argumentValue"/> are considered equal.
        /// </summary>
        /// <param name="expectedValue">The first object to compare.</param>
        /// <param name="argumentValue">The second object to compare.</param>
        /// <returns><c>true</c> if the objects are considered equal. Otherwise <c>false</c>.</returns>
        bool AreEqual(object? expectedValue, object? argumentValue);
    }
}
