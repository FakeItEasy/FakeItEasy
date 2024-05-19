namespace FakeItEasy.Creation;

internal class SuccessfulCreationResult : CreationResult
{
    public SuccessfulCreationResult(object? result) => this.Result = result;

    public override bool WasSuccessful => true;

    public override object? Result { get; }

    public override CreationResult MergeIntoDummyResult(CreationResult other) => this;
}
