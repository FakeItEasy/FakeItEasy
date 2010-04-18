namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents a definition of how fakes of the specified type should be created.
    /// </summary>
    public interface IFakeDefinition
    {
        /// <summary>
        /// The type of fake object the definition is for.
        /// </summary>
        Type ForType { get; }

        /// <summary>
        /// Creates the fake.
        /// </summary>
        /// <returns>The fake object.</returns>
        object CreateFake();
    }
}
