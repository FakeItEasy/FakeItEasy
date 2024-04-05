namespace FakeItEasy.Core;

/// <summary>
/// A fake object call rule that is stateful and provides a method to get a
/// snapshot of the rule.
/// </summary>
public interface IStatefulFakeObjectCallRule : IFakeObjectCallRule
{
    /// <summary>
    /// Gets a snapshot of the rule's current state, to be able to restore the
    /// rule when the fake is reset.
    /// </summary>
    /// <returns>A snapshot of the rule's current state.</returns>
    IFakeObjectCallRule GetSnapshot();
}
