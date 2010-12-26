namespace FakeItEasy.Core
{
    using System;

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
        /// Applies base configuration to a fake object.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeObject">The fake object to configure.</param>
        public void ConfigureFake(Type typeOfFake, object fakeObject)
        {
        }
    }
}