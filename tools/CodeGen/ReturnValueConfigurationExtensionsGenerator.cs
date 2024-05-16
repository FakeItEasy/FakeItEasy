namespace CodeGen;

using Microsoft.CodeAnalysis;

[Generator]
public class ReturnValueConfigurationExtensionsGenerator : StronglyTypedOverloadsGenerator
{
    protected override bool IsApplicable(GeneratorExecutionContext context)
    {
        return IsTypeInTargetAssembly(context, "FakeItEasy.ReturnValueConfigurationExtensions");
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

                public static partial class ReturnValueConfigurationExtensions
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
            /// Specifies a function used to produce a return value when the configured call is made.
            /// The function will be called each time this call is made and can return different values
            /// each time.
            /// </summary>
            /// <typeparam name="TReturnType">The type of the return value.</typeparam>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <param name="configuration">The configuration to extend.</param>
            /// <param name="valueProducer">A function that produces the return value.</param>
            /// <returns>The configuration object.</returns>
            /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
            public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<TReturnType>>
                ReturnsLazily<TReturnType, {{typeParamList}}>(this IReturnValueConfiguration<TReturnType> configuration, Func<{{typeParamList}}, TReturnType> valueProducer)
            {
                Guard.AgainstNull(configuration);
                Guard.AgainstNull(valueProducer);

                return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                    return valueProducer({{ArgumentsFromCall(typeParametersCount)}});
                });
            }

            /// <summary>
            /// Specifies a function used to produce the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/> which is returned when the configured call is made.
            /// The function will be called each time the configured call is made and can return different values each time.
            /// The <see cref="Task{T}"/> returned from the configured call will have a <see cref="Task.Status"/> of <see cref="TaskStatus.RanToCompletion"/>.
            /// </summary>
            /// <typeparam name="TReturnType">The type of the result produced by the <see cref="Task{T}"/>.</typeparam>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <param name="configuration">The configuration to extend.</param>
            /// <param name="valueProducer">A function that produces the <see cref="Task{T}.Result"/> of the <see cref="Task{T}"/>.</param>
            /// <returns>The configuration object.</returns>
            /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="valueProducer"/> do not match.</exception>
            public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<TReturnType>>>
                ReturnsLazily<TReturnType, {{typeParamList}}>(this IReturnValueConfiguration<Task<TReturnType>> configuration, Func<{{typeParamList}}, TReturnType> valueProducer)
            {
                Guard.AgainstNull(configuration);
                Guard.AgainstNull(valueProducer);

                return configuration.ReturnsLazily(call =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, valueProducer.GetMethodInfo(), NameOfReturnsLazilyFeature);

                    return TaskHelper.FromResult(valueProducer({{ArgumentsFromCall(typeParametersCount)}}));
                });
            }
            """;
    }
}
