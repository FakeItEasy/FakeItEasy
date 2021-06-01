namespace FakeItEasy.Configuration
{
    internal static class AnyCallConfigurationWithNoReturnTypeSpecifiedExtensions
    {
        public static IAnyCallConfigurationWithVoidReturnType MatchingEventAction(
            this IAnyCallConfigurationWithNoReturnTypeSpecified configuration,
            object fake,
            EventAction action)
        {
            Guard.AgainstNull(configuration, nameof(configuration));
            Guard.AgainstNull(fake, nameof(fake));
            Guard.AgainstNull(action, nameof(action));
            return configuration.WithVoidReturnType().Where(action.Matches, writer => action.WriteDescription(fake, writer));
        }
    }
}
