namespace FakeItEasy
{
    using System;
    using FakeItEasy.Creation;

    /// <summary>
    /// Builds options to be used during Fake creation.
    /// </summary>
    public interface IFakeOptionsBuilder
    {
        /// <summary>
        /// Gets the priority of the options builder. When multiple builders that apply to
        /// the same type are registered, the one with the highest priority value is used.
        /// </summary>
        Priority Priority { get; }

        /// <summary>
        /// Whether or not this object can build options for a Fake of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of fake to build options for.</param>
        /// <returns>
        /// <c>true</c> if this object can build options for a
        /// Fake of type <paramref name="type"/>. Otherwise <c>false</c>.
        /// </returns>
        bool CanBuildOptionsForFakeOfType(Type type);

        /// <summary>
        /// Manipulates <paramref name="options"/>, which will later be used to
        /// create a Fake.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="options">The fake options to manipulate.</param>
        void BuildOptions(Type typeOfFake, IFakeOptions options);
    }
}
