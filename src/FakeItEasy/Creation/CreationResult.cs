namespace FakeItEasy.Creation
{
    internal abstract class CreationResult
    {
        public abstract bool WasSuccessful { get; }

        public abstract object? Result { get; }
    }
}
