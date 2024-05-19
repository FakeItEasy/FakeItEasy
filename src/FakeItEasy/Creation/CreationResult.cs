namespace FakeItEasy.Creation
{
    internal abstract class CreationResult
    {
        public static CreationResult Untried { get; } = new UntriedCreationResult();

        public abstract bool WasSuccessful { get; }

        public abstract object? Result { get; }

        /// <summary>
        /// Returns a creation result for a dummy by combining two results.
        /// Successful results are preferred to failed. Failed results will have their reasons for failure aggregated.
        /// </summary>
        /// <param name="other">The other result to merge. Must not be <c>null</c>.</param>
        /// <returns>A combined creation result. Successful if either input was successful, and failed otherwise.</returns>
        public abstract CreationResult MergeIntoDummyResult(CreationResult other);
    }
}
