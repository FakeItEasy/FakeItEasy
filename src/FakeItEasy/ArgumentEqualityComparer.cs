namespace FakeItEasy;

using System;

/// <summary>
/// Provides custom equality comparison for call arguments for parameters of type T.
/// </summary>
/// <typeparam name="T">The type of parameter to compare.</typeparam>
public abstract class ArgumentEqualityComparer<T> : IArgumentEqualityComparer
{
    /// <summary>
    /// Gets the priority of the argument equality comparer. When multiple comparers that apply to the same type are registered,
    /// the one with the highest priority value is used.
    /// </summary>
    public virtual Priority Priority => Priority.Default;

    /// <summary>
    /// Whether or not this object can compare instances of <paramref name="type"/> for equality.
    /// </summary>
    /// <param name="type">The type of the objects to compare.</param>
    /// <returns>
    /// <c>true</c> if this object can compare instances of <paramref name="type"/>. Otherwise <c>false</c>.
    /// </returns>
    public virtual bool CanCompare(Type type) => type == typeof(T);

    /// <summary>
    /// Indicates whether <paramref name="expectedValue"/> and <paramref name="argumentValue"/> are considered equal.
    /// </summary>
    /// <param name="expectedValue">The first object to compare.</param>
    /// <param name="argumentValue">The second object to compare.</param>
    /// <returns><c>true</c> if the objects are considered equal. Otherwise <c>false</c>.</returns>
    /// <remarks>This method cannot be overridden. Override the <see cref="AreEqual(T, T)"/> method instead.</remarks>
    public bool AreEqual(object expectedValue, object argumentValue) => this.AreEqual((T)expectedValue, (T)argumentValue);

    /// <summary>
    /// When overridden in a derived class, indicates whether <paramref name="expectedValue"/> and
    /// <paramref name="argumentValue"/> are considered equal.
    /// </summary>
    /// <param name="expectedValue">The first object to compare.</param>
    /// <param name="argumentValue">The second object to compare.</param>
    /// <returns><c>true</c> if the objects are considered equal. Otherwise <c>false</c>.</returns>
    protected abstract bool AreEqual(T expectedValue, T argumentValue);
}
