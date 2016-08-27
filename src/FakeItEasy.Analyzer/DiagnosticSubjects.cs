namespace FakeItEasy.Analyzer
{
    internal static class DiagnosticSubjects
    {
        internal static string[] CallSpecMemberNames()
        {
            var callSpecMemberNames = new[]
            {
                "FakeItEasy.A.CallTo",
                "FakeItEasy.A.CallTo`1",
                "FakeItEasy.A.CallToSet`1",
                "FakeItEasy.Fake`1.CallsTo`1",
                "FakeItEasy.Fake`1.AnyCall",
                "FakeItEasy.ArgumentValidationConfigurationExtensions.WithAnyArguments`1",
                "FakeItEasy.WhereConfigurationExtensions.Where`1",
                "FakeItEasy.Configuration.IAnyCallConfigurationWithNoReturnTypeSpecified.WithReturnType`1",
                "FakeItEasy.Configuration.IAnyCallConfigurationWithNoReturnTypeSpecified.WithNonVoidReturnType",
                "FakeItEasy.Configuration.IArgumentValidationConfiguration`1.WhenArgumentsMatch",
                "FakeItEasy.Configuration.IPropertySetterAnyValueConfiguration`1.To",
                "FakeItEasy.Configuration.IWhereConfiguration`1.Where"
            };

            return callSpecMemberNames;

        }
    }
}
