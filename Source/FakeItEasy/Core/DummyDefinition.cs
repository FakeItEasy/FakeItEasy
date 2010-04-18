namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents a definition of how a fake object of the type T should
    /// be created.
    /// </summary>
    /// <typeparam name="T">The type of fake.</typeparam>
    public abstract class DummyDefinition<T>
        : IDummyDefinition
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
        object IDummyDefinition.CreateFake()
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