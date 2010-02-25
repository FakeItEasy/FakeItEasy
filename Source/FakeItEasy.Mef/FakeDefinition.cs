namespace FakeItEasy.Mef
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    public abstract class FakeDefinition<T>
        : IFakeDefinition
    {
        public Type ForType
        {
            get { return typeof(T); }
        }

        object IFakeDefinition.CreateFake()
        {
            return this.CreateFake();
        }

        protected abstract T CreateFake();
    }
}