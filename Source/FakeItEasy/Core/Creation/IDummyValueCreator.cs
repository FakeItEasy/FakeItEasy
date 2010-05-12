namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    /// <summary>
    /// Responsible for creating dummy values.
    /// </summary>
    public interface IDummyValueCreator
    {
        /// <summary>
        /// Tries to create a dummy value of the specified type.
        /// </summary>
        /// <param name="type">The type of dummy to create.</param>
        /// <param name="dummy">An output parameter for the result.</param>
        /// <returns>True if a dummy could be created.</returns>
        bool TryCreateDummyValue(Type type, out object dummy);
    }
}