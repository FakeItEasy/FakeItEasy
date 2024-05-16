namespace CodeGen;

using Microsoft.CodeAnalysis;

[Generator]
public class CallbackConfigurationExtensionsGenerator : StronglyTypedOverloadsGenerator
{
    protected override bool IsApplicable(GeneratorExecutionContext context)
    {
        return IsTypeInTargetAssembly(context, "FakeItEasy.CallbackConfigurationExtensions");
    }

    protected override string GenerateSource()
    {
        return $$"""
            namespace FakeItEasy
            {
                using System;
                using System.Reflection;
                using FakeItEasy.Configuration;

                public static partial class CallbackConfigurationExtensions
                {
            {{Indent(2, OverloadMethod(GenerateMethodImplementation))}}
                }
            }
            """;
    }

    private static string GenerateMethodImplementation(int typeParametersCount)
    {
        var typeParamList = TypeParamList(typeParametersCount);
        return $$"""
            /// <summary>
            /// Executes the specified action when a matching call is being made.
            /// </summary>
            /// <param name="configuration">The configuration that is extended.</param>
            /// <param name="actionToInvoke">The <see cref="Action{{{typeParamList}}}"/> to invoke.</param>
            /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="actionToInvoke"/> do not match.</exception>
            /// <returns>The configuration object.</returns>
            public static TInterface Invokes<TInterface, {{typeParamList}}>(this ICallbackConfiguration<TInterface> configuration, Action<{{typeParamList}}> actionToInvoke)
            {
                Guard.AgainstNull(configuration);
                Guard.AgainstNull(actionToInvoke);

                return configuration.Invokes(call =>
                    {
                        ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, actionToInvoke.GetMethodInfo(), NameOfInvokesFeature);

                        actionToInvoke({{ArgumentsFromCall(typeParametersCount)}});
                    });
            }
            """;
    }
}
