namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A base implementation for factories for creating fake objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of fake.</typeparam>
    public abstract class DummyFactory<T> : IDummyFactory
    {
        /// <summary>
        /// Gets the priority of the dummy factory. When multiple factories that apply to the same type are registered,
        /// the one with the highest priority is used.
        /// </summary>
        /// <remarks>The default implementation returns <see cref="FakeItEasy.Priority.Default"/>.</remarks>
        public virtual Priority Priority => Priority.Default;

        /// <summary>
        /// Whether or not this object can create a dummy of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of dummy to create.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> is <typeparamref name="T"/>. Otherwise <c>false</c>.
        /// </returns>
        public bool CanCreate(Type type)
        {
            return type == typeof(T);
        }

        /// <summary>
        /// Creates the dummy.
        /// </summary>
        /// <param name="type">The type of dummy to create.</param>
        /// <returns>The dummy object. Unlike a dummy provided by built-in FakeItEasy mechanisms, may be <c>null</c>.</returns>
        /// <exception cref="ArgumentException"><paramref name="type"/> is not <typeparamref name="T"/>.</exception>
        public object? Create(Type type)
        {
            this.AssertThatFakeIsOfCorrectType(type);

            return this.Create();
        }

        /// <summary>
        /// Creates the dummy.
        /// </summary>
        /// <returns>The dummy object. Unlike a dummy provided by built-in FakeItEasy mechanisms, may be <c>null</c>.</returns>
        protected abstract T Create();

        private void AssertThatFakeIsOfCorrectType(Type type)
        {
            if (type != typeof(T))
            {
                var message = $"The {this.GetType()} can only create dummies of type {typeof(T)}.";

                throw new ArgumentException(message, nameof(type));
            }
        }
    }
}
