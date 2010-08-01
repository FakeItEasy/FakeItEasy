namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// Provides the base implementation for the IFakeConfigurator-interface.
    /// </summary>
    /// <typeparam name="T">The type of fakes the configurator can configure.</typeparam>
    public abstract class FakeConfigurer<T>
        : IFakeConfigurer
    {
        /// <summary>
        /// The type the instance provides configuration for.
        /// </summary>
        /// <value></value>
        public Type ForType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Applies the configuration for the specified fake object.
        /// </summary>
        /// <param name="fakeObject">The fake object to configure.</param>
        void IFakeConfigurer.ConfigureFake(object fakeObject)
        {
            Guard.AgainstNull(fakeObject, "fakeObject");

            this.AssertThatFakeIsOfCorrectType(fakeObject);

            this.ConfigureFake((T)fakeObject);
        }

        /// <summary>
        /// Asserts the type of the that fake is of correct.
        /// </summary>
        /// <param name="fakeObject">The fake object.</param>
        private void AssertThatFakeIsOfCorrectType(object fakeObject)
        {
            if (!(fakeObject is T))
            { 
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The {0} can only configure fakes of the type '{1}'.",
                    this.GetType(), typeof(T)), "fakeObject");
            }
        }

        /// <summary>
        /// Configures the fake.
        /// </summary>
        /// <param name="fakeObject">The fake object.</param>
        public abstract void ConfigureFake(T fakeObject);
    }
}
