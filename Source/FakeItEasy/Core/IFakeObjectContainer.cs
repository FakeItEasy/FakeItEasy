namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A container that can create fake objects.
    /// </summary>
    public interface IFakeObjectContainer
        : IFakeObjectConfigurator
    {
        /// <summary>
        /// Creates a dummy object of the specified type using the specified arguments if it's
        /// supported by the container, returns a value indicating if it's supported or not.
        /// </summary>
        /// <param name="typeOfDummy">The type of dummy object to create.</param>
        /// <param name="fakeObject">The dummy object that was created if the method returns true.</param>
        /// <returns>True if a dummy object can be created.</returns>
        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Used by the framework, generics would have no benefit.")]
        bool TryCreateDummyObject(Type typeOfDummy, out object fakeObject);
    }
}