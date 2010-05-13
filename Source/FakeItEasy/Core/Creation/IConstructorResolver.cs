namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Resolves constructors of types.
    /// </summary>
    public interface IConstructorResolver
    {
        /// <summary>
        /// Gets all the accessible constructor for the type along with dummy arguments
        /// where they can be resolved.
        /// </summary>
        /// <param name="type">The type to list constructors for.</param>
        /// <returns>A collection of constructors.</returns>
        IEnumerable<ConstructorAndArgumentsInfo> ListAllConstructors(Type type);
    }
}
