namespace FakeItEasy.IntegrationTests.External
{
    using System;
    using FakeItEasy;

    public class GuidValueFormatter : ArgumentValueFormatter<Guid>
    {
        protected override string GetStringValue(Guid value)
        {
            return value.ToString("B");
        }
    }
}
