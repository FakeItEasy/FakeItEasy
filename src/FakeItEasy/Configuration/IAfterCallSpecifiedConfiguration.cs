namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Lets you set up expectations and configure repeat for the configured call.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IAfterCallSpecifiedConfiguration<out TInterface>
        : IRepeatConfiguration<TInterface>
    {
    }
}
