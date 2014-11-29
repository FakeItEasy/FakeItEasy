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
        /// Gets the priority of the dummy definition. When multiple definitions that
        /// apply to the same type are registered, the one with the highest
        /// priority is used.
        /// </summary>
        public virtual int Priority
        {
            get { return 0; }
        }

        /// <summary>
        /// Whether or not this object can create a dummy of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of dummy to create.</param>
        /// <returns><c>true</c> if we can create a dummy of type <paramref name="type"/>. Otherwise <c>false</c>.</returns>
        public bool CanCreateDummyOfType(Type type)
        {
            return type == typeof(T);
        }

        /// <summary>
        /// Creates the dummy.
        /// </summary>
        /// <param name="type">The type of dummy to create.</param>
        /// <returns>The dummy object.</returns>
        public object CreateDummyOfType(Type type)
        {
//// Guard against bad type
            return this.CreateDummy();
        }

        /// <summary>
        /// Creates the dummy.
        /// </summary>
        /// <returns>The dummy object.</returns>
        protected abstract T CreateDummy();
    }
}