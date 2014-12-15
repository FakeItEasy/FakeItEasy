namespace FakeItEasy
{
    using System;

    /// <summary>
    /// Represents a definition of how dummies of the specified type should be created.
    /// </summary>
    public interface IDummyDefinition
    {
        /// <summary>
        /// Gets the priority of the dummy definition. When multiple definitions that
        /// apply to the same type are registered, the one with the highest
        /// priority is used.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Whether or not this object can create a dummy of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of dummy to create.</param>
        /// <returns><c>true</c> if we can create a dummy of type <paramref name="type"/>. Otherwise <c>false</c>.</returns>
        bool CanCreateDummyOfType(Type type);

        /// <summary>
        /// Creates the dummy.
        /// </summary>
        /// <param name="type">The type of dummy to create.</param>
        /// <returns>The dummy object.</returns>
        object CreateDummyOfType(Type type);
    }
}