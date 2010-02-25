namespace FakeItEasy
{
    /// <summary>
    /// Represents a class that's responsible for generating fake objects.
    /// </summary>
    internal interface IFakeObjectGenerator
    {
        /// <summary>
        /// Tries to generate a fake object.
        /// </summary>
        /// <returns>A value indicating if the generation was successful.</returns>
        bool GenerateFakeObject();

        /// <summary>
        /// The result of the last generation.
        /// </summary>
        object GeneratedFake { get; }
    }
}
