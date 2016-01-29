namespace FakeItEasy.Specs
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Xbehave;

    public static class ArgumentValueFormatterSpecs
    {
        [Scenario]
        public static void GenericArgumentValueFormatterDefaultPriority(
            IArgumentValueFormatter formatter,
            int priority)
        {
            "Given an argument value formatter that extends the generic base"
                .x(() => formatter = new SomeArgumentValueFormatter());

            "When I fetch the Priority"
                .x(() => priority = formatter.Priority);

            "Then it should be 0"
                .x(() => priority.Should().Be(0));
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Required for testing.")]
        private class SomeClass
        {
        }

        private class SomeArgumentValueFormatter : ArgumentValueFormatter<SomeClass>
        {
            protected override string GetStringValue(SomeClass argumentValue)
            {
                return "formatted SomeClass";
            }
        }
    }
}
