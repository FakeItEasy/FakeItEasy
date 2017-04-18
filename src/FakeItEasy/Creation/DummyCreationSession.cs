namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;

    internal class DummyCreationSession
    {
        private readonly HashSet<Type> typesCurrentlyBeingResolved = new HashSet<Type>();

        public bool TryBeginToResolveType(Type typeOfDummy)
        {
            if (this.typesCurrentlyBeingResolved.Contains(typeOfDummy))
            {
                return false;
            }

            this.typesCurrentlyBeingResolved.Add(typeOfDummy);
            return true;
        }

        public void OnSuccessfulResolve(Type typeOfDummy)
        {
            this.typesCurrentlyBeingResolved.Remove(typeOfDummy);
        }
    }
}
