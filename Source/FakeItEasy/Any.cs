namespace FakeItEasy
{
    using System.ComponentModel;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides configuration for any (not a specific) call on a faked object.
    /// </summary>
    public static class Any
    {
        /// <summary>
        /// Gets a configuration object allowing for further configuration of
        /// any calll to the specified faked object.
        /// </summary>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <param name="fakedObject">The faked object to configure.</param>
        /// <returns>A configuration object.</returns>
        public static IAnyCallConfiguration CallTo<TFake>(TFake fakedObject)
        {
            var configurationFactory = ServiceLocator.Current.Resolve<IStartConfigurationFactory>();
            return configurationFactory.CreateConfiguration<TFake>(Fake.GetFakeObject(fakedObject)).AnyCall();
        }

        /// <summary>
        /// Gets a value indicating if the two objects are equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>True if the two objects are equal.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object objA, object objB)
        {
            return object.Equals(objA, objB);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are the same reference.
        /// </summary>
        /// <param name="objA">The obj A.</param>
        /// <param name="objB">The obj B.</param>
        /// <returns>True if the objects are the same reference.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool ReferenceEquals(object objA, object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }
    }
}
