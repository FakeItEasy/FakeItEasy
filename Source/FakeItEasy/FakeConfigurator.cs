namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// Provides the base implementation for the IFakeConfigurator-interface.
    /// </summary>
    /// <typeparam name="T">The type of fakes the configurator can configure.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configurator", Justification = "This is the correct spelling.")]
    public abstract class FakeConfigurator<T>
        : IFakeConfigurator
    {
        /// <summary>
        /// Gets the priority of the fake configurator. When multiple configurators that
        /// apply to the same type are registered, the one with the highest
        /// priority is used.
        /// </summary>
        /// <remarks>The default implementation returns <c>0</c>.</remarks>
        public virtual int Priority
        {
            get { return 0; }
        }

        /// <summary>
        /// Whether or not this object can configure a fake of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of fake to configure.</param>
        /// <returns><c>true</c> if <paramref name="type"/> is <typeparamref name="T"/>. Otherwise <c>false</c>.</returns>
        public bool CanConfigureFakeOfType(Type type)
        {
            return type == typeof(T);
        }

        /// <summary>
        /// Applies the configuration for the specified fake object.
        /// </summary>
        /// <param name="fakeObject">The fake object to configure.</param>
        void IFakeConfigurator.ConfigureFake(object fakeObject)
        {
            Guard.AgainstNull(fakeObject, "fakeObject");

            this.AssertThatFakeIsOfCorrectType(fakeObject);

            this.ConfigureFake((T)fakeObject);
        }

        /// <summary>
        /// Configures the fake.
        /// </summary>
        /// <param name="fakeObject">The fake object.</param>
        protected abstract void ConfigureFake(T fakeObject);

        private void AssertThatFakeIsOfCorrectType(object fakeObject)
        {
            if (!(fakeObject is T))
            {
                var message = string.Format(
                                     CultureInfo.InvariantCulture, 
                                     "The {0} can only configure fakes of type '{1}'.", 
                                     this.GetType(), 
                                     typeof(T));
                throw new ArgumentException(message, "fakeObject");
            }
        }
    }
}