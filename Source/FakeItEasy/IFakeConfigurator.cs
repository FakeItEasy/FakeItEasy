namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides configuration for certain types of fake objects.
    /// </summary>
    public interface IFakeConfigurator
    {
        /// <summary>
        /// Gets the priority of the fake configurator. When multiple configurators that
        /// apply to the same type are registered, the one with the highest
        /// priority is used.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Whether or not this object can configure a fake of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of fake to configure.</param>
        /// <returns><c>true</c> if this object can configure a fake of type <paramref name="type"/>. Otherwise <c>false</c>.</returns>
        bool CanConfigureFakeOfType(Type type);

        /// <summary>
        /// Applies the configuration for the specified fake object.
        /// </summary>
        /// <param name="fakeObject">The fake object to configure.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Fake object is a common term in FakeItEasy.")]
        void ConfigureFake(object fakeObject);
    }
}