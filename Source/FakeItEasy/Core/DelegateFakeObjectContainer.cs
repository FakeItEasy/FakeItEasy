namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;

    public class DelegateFakeObjectContainer
        : IFakeObjectContainer
    {
        private Dictionary<Type, Func<object>> registeredDelegates;

        public DelegateFakeObjectContainer()
        {
            this.registeredDelegates = new Dictionary<Type, Func<object>>();
        }

        public bool TryCreateFakeObject(Type typeOfFakeObject, out object fakeObject)
        {
            Func<object> creator = null;

            if (!this.registeredDelegates.TryGetValue(typeOfFakeObject, out creator))
            {
                fakeObject = null;
                return false;
            }

            fakeObject = creator.Invoke();
            return true;
        }
        
        public void Register<T>(Func<T> fakeDelegate)
        {
            this.registeredDelegates[typeof(T)] = () => fakeDelegate.Invoke();
        }


        public void ConfigureFake(Type typeOfFake, object fakeObject)
        {
            
        }
    }
}
