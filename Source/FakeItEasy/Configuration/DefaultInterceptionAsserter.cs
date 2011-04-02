namespace FakeItEasy.Configuration
{
    using System.Reflection;
    using System.Text;
    using Creation;

    internal class DefaultInterceptionAsserter
        : IInterceptionAsserter
    {
        private readonly IProxyGenerator proxyGenerator;

        public DefaultInterceptionAsserter(IProxyGenerator proxyGenerator)
        {
            this.proxyGenerator = proxyGenerator;
        }

        public void AssertThatMethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget)
        {
            string failReason;
            if (!this.proxyGenerator.MethodCanBeInterceptedOnInstance(method, callTarget, out failReason))
            {
                var message = new StringBuilder()
                    .AppendLine()
                    .AppendLine()
                    .Append("  ")
                    .AppendLine(
                        "The current proxy generator can not intercept the specified method for the following reason:")
                    .Append("    - ")
                    .AppendLine(failReason)
                    .AppendLine().ToString();

                throw new FakeConfigurationException(message);
            }
        }
    }
}