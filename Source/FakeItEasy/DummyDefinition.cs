namespace FakeItEasy
{
    using System;

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
        /// Creates the dummy.
        /// </summary>
        /// <returns>The dummy object.</returns>
        object IDummyDefinition.CreateDummy()
        {
            return this.CreateDummy();
        }

        /// <summary>
        /// Creates the dummy.
        /// </summary>
        /// <returns>The dummy object.</returns>
        protected abstract T CreateDummy();
    }
}