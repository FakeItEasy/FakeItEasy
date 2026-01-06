namespace FakeItEasy.Expressions.ArgumentConstraints;

using FakeItEasy;
using FakeItEasy.Core;

internal class NullArgumentConstraint
    : IArgumentConstraint
{
    private NullArgumentConstraint()
    {
    }

    public static NullArgumentConstraint Instance { get; } = new NullArgumentConstraint();

    public string ConstraintDescription => this.ToString();

    public bool IsValid(object? argument) => argument is null;

    public override string ToString() => "NULL";

    public void WriteDescription(IOutputWriter writer) => writer.Write(this.ToString());
}
