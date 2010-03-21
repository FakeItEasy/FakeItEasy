namespace FakeItEasy.Mef
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Represents a definition of how a fake object of the type T should
    /// be created.
    /// </summary>
    /// <typeparam name="T">The type of fake.</typeparam>
    public abstract class FakeDefinition<T>
        : IFakeDefinition
    {
        /// <summary>
        /// Gets the type the definition is for.
        /// </summary>
        /// <value>For type.</value>
        public Type ForType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Creates the fake.
        /// </summary>
        /// <returns>The fake object.</returns>
        object IFakeDefinition.CreateFake()
        {
            return this.CreateFake();
        }

        /// <summary>
        /// Creates the fake.
        /// </summary>
        /// <returns>The fake object.</returns>
        protected abstract T CreateFake();
    }
}