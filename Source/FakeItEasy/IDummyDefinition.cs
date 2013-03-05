namespace FakeItEasy
{
    using System;
    
    /// <summary>
    /// Represents a definition of how dummies of the specified type should be created.
    /// </summary>
    public interface IDummyDefinition
    {
        /// <summary>
        /// Gets the type of fake object the definition is for.
        /// </summary>
        Type ForType { get; }

        /// <summary>
        /// Creates the fake.
        /// </summary>
        /// <returns>The fake object.</returns>
        object CreateDummy();
    }
}