namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Lets you set up expectations and configure repeat for the configured call.
    /// </summary>
    public interface IAfterCallSpecifiedConfiguration<out TInterface>
        : IRepeatConfiguration<TInterface>
    {
    }
}
