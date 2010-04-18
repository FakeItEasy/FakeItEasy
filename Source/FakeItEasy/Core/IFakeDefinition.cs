namespace FakeItEasy.Mef
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Represents a definition of how fakes of the specified type should be created.
    /// </summary>
    [InheritedExport(typeof(IFakeDefinition))]
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
