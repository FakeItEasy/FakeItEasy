namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Allows the developer to assert on a call that's configured.
    /// </summary>
    public interface IAssertConfiguration
        : IHideObjectMembers
    {
        void MustHaveHappened(Repeated repeatConstraint);
    }
}
