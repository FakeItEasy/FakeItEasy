namespace CodeGen;

using Microsoft.CodeAnalysis;

[Generator]
public class OutAndRefParametersGenerator : StronglyTypedOverloadsGenerator
{
    protected override bool IsApplicable(GeneratorExecutionContext context)
    {
        return IsTypeInTargetAssembly(context, "FakeItEasy.Configuration.IOutAndRefParametersConfiguration`1");
    }

    protected override string GenerateSource()
    {
        return $$"""
            #nullable enable
            namespace FakeItEasy.Configuration
            {
                using System;
                using System.Reflection;

                public partial interface IOutAndRefParametersConfiguration<out TInterface>
                {
            {{Indent(2, OverloadMethod(GenerateMethodDeclaration))}}
                }

                internal partial class RuleBuilder
                {
                    private const string NameOfOutRefLazilyFeature = "assigns out and ref parameters lazily";

            {{Indent(2, GenerateMethodImplementations("IVoidConfiguration"))}}

                    public partial class ReturnValueConfiguration<TMember>
                    {
            {{Indent(3, GenerateMethodImplementations("IReturnValueConfiguration<TMember>"))}}
                    }
                }

                internal partial class AnyCallConfiguration
                {
                    private const string NameOfOutRefLazilyFeature = "assigns out and ref parameters lazily";

            {{Indent(2, GenerateMethodImplementations("IVoidConfiguration"))}}
                }
            }
            """;
    }

    private static string GenerateMethodDeclaration(int typeParametersCount)
    {
        var typeParamList = TypeParamList(typeParametersCount);
        return $"""
            /// <summary>
            /// Specifies a function used to produce output values for out and ref parameters.
            /// The values should appear in the same order as the out and ref parameters in the configured call.
            /// Any non out and ref parameters are ignored.
            /// The function will be called each time this call is made and can return different values.
            /// </summary>
            {TypeParamDocsSource(typeParametersCount)}
            /// <param name="valueProducer">A function that produces the output values.</param>
            /// <returns>A configuration object.</returns>
            /// <exception cref="FakeConfigurationException">
            /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
            /// </exception>
            IAfterCallConfiguredConfiguration<TInterface> AssignsOutAndRefParametersLazily<{typeParamList}>(Func<{typeParamList}, object?[]> valueProducer);
            """;
    }

    private static string GenerateMethodImplementations(string configurationInterface) =>
        OverloadMethod(i => GenerateMethodImplementation(configurationInterface, i));

    private static string GenerateMethodImplementation(string configurationInterface, int typeParametersCount)
    {
        var typeParamList = TypeParamList(typeParametersCount);
        return $$"""
            /// <summary>
            /// Specifies a function used to produce output values for out and ref parameters.
            /// The values should appear in the same order as the out and ref parameters in the configured call.
            /// Any non out and ref parameters are ignored.
            /// The function will be called each time this call is made and can return different values.
            /// </summary>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <param name="valueProducer">A function that produces the output values.</param>
            /// <returns>A configuration object.</returns>
            /// <exception cref="FakeConfigurationException">
            /// The signatures of the faked method and the <paramref name="valueProducer"/> do not match.
            /// </exception>
            public IAfterCallConfiguredConfiguration<{{configurationInterface}}> AssignsOutAndRefParametersLazily<{{typeParamList}}>(Func<{{typeParamList}}, object?[]> valueProducer)
            {
                Guard.AgainstNull(valueProducer);

                return this.AssignsOutAndRefParametersLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(
                        call.Method, valueProducer.GetMethodInfo(), NameOfOutRefLazilyFeature);

                    return valueProducer({{ArgumentsFromCall(typeParametersCount)}});
                });
            }
            """;
    }
}
