namespace FakeItEasy
{
    using System;

    /// <summary>
    /// Represents a factory for creating dummies of certain types.
    /// </summary>
    public interface IDummyFactory
    {
        /// <summary>
        /// Gets the priority of the dummy factory. When multiple factories that apply to the same type are registered,
        /// the one with the highest priority value is used.
        /// </summary>
        Priority Priority { get; }

        /// <summary>
        /// Whether or not this object can create a dummy of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of dummy to create.</param>
        /// <returns>
        /// <c>true</c> if the object can create a dummy of type <paramref name="type"/>. Otherwise <c>false</c>.
        /// </returns>
        bool CanCreate(Type type);

        /// <summary>
        /// Creates the dummy.
        /// </summary>
        /// <param name="type">The type of dummy to create.</param>
        /// <returns>The dummy object. Unlike a dummy provided by built-in FakeItEasy mechanisms, may be <c>null</c>.</returns>
        object? Create(Type type);
    }
}
