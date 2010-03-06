namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A null implementation for the IFakeObjectContainer interface.
    /// </summary>
    public class NullFakeObjectContainer
        : IFakeObjectContainer
    {
        /// <summary>
        /// Always returns false and sets the fakeObject to null.
        /// </summary>
        /// <param name="typeOfFakeObject">The type of fake object to create.</param>
        /// <param name="arguments">Arguments for the fake object, ignored.</param>
        /// <param name="fakeObject">Output variable for the fake object that will always be set to null.</param>
        /// <returns>Always return false.</returns>
        public bool TryCreateFakeObject(Type typeOfFakeObject, out object fakeObject)
        {
            fakeObject = null;
            return false;
        }


        public void ConfigureFake(Type typeOfFakeObject, object fakeObject)
        {

        }
    }
}
