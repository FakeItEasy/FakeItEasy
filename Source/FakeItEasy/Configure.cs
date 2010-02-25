namespace FakeItEasy
{
    using System;
    using FakeItEasy.Configuration;
    using System.Linq.Expressions;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides configuration of faked objects.
    /// </summary>
    public static class Configure
    {
        /// <summary>
        /// Gets a configuration for the specified faked object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="fakedObject">The faked object to configure.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="ArgumentException">The specified object is not a faked object.</exception>
        /// <exception cref="ArgumentNullException">The fakedObject parameter was null.</exception>
        [Obsolete]
        public static IStartConfiguration<TFake> Fake<TFake>(TFake fakedObject)
        {
            Guard.IsNotNull(fakedObject, "fakedObject");
            
            var factory = ServiceLocator.Current.Resolve<IStartConfigurationFactory>();
            return factory.CreateConfiguration<TFake>(FakeItEasy.Fake.GetFakeObject(fakedObject));
        }
    }
}
