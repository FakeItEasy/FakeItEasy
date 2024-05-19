namespace FakeItEasy.Creation;

using System;

internal class UntriedCreationResult : CreationResult
{
    public override bool WasSuccessful => false;

    public override object Result => throw new NotSupportedException();

    public override CreationResult MergeIntoDummyResult(CreationResult other) => other;
}
