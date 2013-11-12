namespace FakeItEasy.IntegrationTests.External
{
    using System;
    using FakeItEasy;

    public class GuidValueFormatter : ArgumentValueFormatter<Guid>
    {
        protected override string GetStringValue(Guid argumentValue)
        {
            return argumentValue.ToString("B");
        }
    }
}
