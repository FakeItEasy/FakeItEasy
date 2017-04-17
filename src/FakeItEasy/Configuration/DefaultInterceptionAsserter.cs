namespace FakeItEasy.Configuration
{
    using System.Reflection;
    using System.Text;
    using FakeItEasy.Creation;

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
                string memberType = method.IsPropertyGetterOrSetter() ? "property" : "method";
                string description = method.GetDescription();
                var message = new StringBuilder()
                    .AppendLine()
                    .AppendLine()
                    .Append("  ")
                    .AppendLine(
                        $"The current proxy generator can not intercept the {memberType} {description} for the following reason:")
                    .Append("    - ")
                    .AppendLine(failReason)
                    .AppendLine().ToString();

                throw new FakeConfigurationException(message);
            }
        }
    }
}
