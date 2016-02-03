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
            Priority priority)
        {
            "Given an argument value formatter that does not override priority"
                .x(() => formatter = new SomeArgumentValueFormatter());

            "When I fetch the Priority"
                .x(() => priority = formatter.Priority);

            "Then it should be the default priority"
                .x(() => priority.Should().Be(Priority.Default));
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
