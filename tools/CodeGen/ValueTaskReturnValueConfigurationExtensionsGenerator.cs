namespace CodeGen;

using Microsoft.CodeAnalysis;

[Generator]
public class ValueTaskReturnValueConfigurationExtensionsGenerator : StronglyTypedOverloadsGenerator
{
    protected override bool IsApplicable(GeneratorExecutionContext context)
    {
        return IsTypeInTargetAssembly(context, "FakeItEasy.ValueTaskReturnValueConfigurationExtensions");
    }

    protected override string GenerateSource()
    {
        return $$"""
            namespace FakeItEasy
            {
                using System;
                using System.Reflection;
                using System.Threading.Tasks;
                using FakeItEasy.Configuration;

                public static partial class ValueTaskReturnValueConfigurationExtensions
                {
                    private const string NameOfReturnsLazilyFeature = "returns lazily";

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
            /// Specifies a function used to produce the <see cref="ValueTask{T}.Result"/> of the <see cref="ValueTask{T}"/> which is returned when the configured call is made.
            /// The function will be called each time the configured call is made and can return different values each time.
            /// The <see cref="ValueTask{T}"/> returned from the configured call will have its <see cref="ValueTask{T}.IsCompletedSuccessfully"/> property set to <c>true</c>".
            /// </summary>
            /// <typeparam name="TReturnType">The type of the result produced by the <see cref="ValueTask{T}"/>.</typeparam>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <param name="configuration">The configuration to extend.</param>
            /// <param name="valueProducer">A function that produces the <see cref="ValueTask{T}.Result"/> of the <see cref="ValueTask{T}"/>.</param>
            /// <returns>The configuration object.</returns>
            /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
            public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<ValueTask<TReturnType>>>
                ReturnsLazily<TReturnType, {{typeParamList}}>(this IReturnValueConfiguration<ValueTask<TReturnType>> configuration, Func<{{typeParamList}}, TReturnType> valueProducer)
            {
                Guard.AgainstNull(configuration);
                Guard.AgainstNull(valueProducer);

                return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                    return new ValueTask<TReturnType>(valueProducer({{ArgumentsFromCall(typeParametersCount)}}));
                });
            }
            """;
    }
}
