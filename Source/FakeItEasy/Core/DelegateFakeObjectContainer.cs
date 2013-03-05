namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A fake object container where delegates can be registered that are used to
    /// resolve fake objects.
    /// </summary>
    public class DelegateFakeObjectContainer
        : IFakeObjectContainer
    {
        private readonly Dictionary<Type, Func<object>> registeredDelegates;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateFakeObjectContainer"/> class. 
        /// Creates a new instance of the DelegateFakeObjectContainer.
        /// </summary>
        public DelegateFakeObjectContainer()
        {
            this.registeredDelegates = new Dictionary<Type, Func<object>>();
        }

        /// <summary>
        /// Creates a fake object of the specified type using the specified arguments if it's
        /// supported by the container, returns a value indicating if it's supported or not.
        /// </summary>
        /// <param name="typeOfDummy">The type of dummy object to create.</param>
        /// <param name="fakeObject">The fake object that was created if the method returns true.</param>
        /// <returns>True if a fake object can be created.</returns>
        public bool TryCreateDummyObject(Type typeOfDummy, out object fakeObject)
        {
            Func<object> creator = null;

            if (!this.registeredDelegates.TryGetValue(typeOfDummy, out creator))
            {
                fakeObject = null;
                return false;
            }

            fakeObject = creator.Invoke();
            return true;
        }

        /// <summary>
        /// Configures the fake.
        /// </summary>
        /// <param name="typeOfFake">The type of fake.</param>
        /// <param name="fakeObject">The fake object.</param>
        public void ConfigureFake(Type typeOfFake, object fakeObject)
        {
        }

        /// <summary>
        /// Registers the specified fake delegate.
        /// </summary>
        /// <typeparam name="T">The type of the return value of the method that <paramref name="fakeDelegate"/> encapsulates.</typeparam>
        /// <param name="fakeDelegate">The fake delegate.</param>
        public void Register<T>(Func<T> fakeDelegate)
        {
            this.registeredDelegates[typeof(T)] = () => fakeDelegate.Invoke();
        }
    }
}