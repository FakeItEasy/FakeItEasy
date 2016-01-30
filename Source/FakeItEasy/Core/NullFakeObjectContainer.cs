namespace FakeItEasy.Core
{
    using System;
    using Creation;

    /// <summary>
    /// A null implementation for the IFakeObjectContainer interface.
    /// </summary>
    public class NullFakeObjectContainer
        : IFakeObjectContainer
    {
        /// <summary>
        /// Always returns false and sets the fakeObject to null.
        /// </summary>
        /// <param name="typeOfDummy">The type of dummy object to create.</param>
        /// <param name="fakeObject">Output variable for the fake object that will always be set to null.</param>
        /// <returns>Always return false.</returns>
        public bool TryCreateDummyObject(Type typeOfDummy, out object fakeObject)
        {
            fakeObject = null;
            return false;
        }

        /// <summary>
        /// Configures a fake's creation options.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeOptions">The options to build for the fake's creation.</param>
        public void BuildOptions(Type typeOfFake, IFakeOptions fakeOptions)
        {
        }
    }
}
