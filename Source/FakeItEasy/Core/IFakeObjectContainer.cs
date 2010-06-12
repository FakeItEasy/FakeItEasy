namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A container that can create fake objects.
    /// </summary>
    public interface IFakeObjectContainer
    {
        /// <summary>
        /// Creates a fake object of the specified type using the specified arguments if it's
        /// supported by the container, returns a value indicating if it's supported or not.
        /// </summary>
        /// <param name="typeOfFake">The type of fake object to create.</param>
        /// <param name="fakeObject">The fake object that was created if the method returns true.</param>
        /// <returns>True if a fake object can be created.</returns>
        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Used by the framework, generics would have no benefit.")]
        bool TryCreateFakeObject(Type typeOfFake, out object fakeObject);

        /// <summary>
        /// Applies base configuration to a fake object.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeObject">The fake object to configure.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Fake object is a common term in FakeItEasy.")]
        void ConfigureFake(Type typeOfFake, object fakeObject);
    }
}
