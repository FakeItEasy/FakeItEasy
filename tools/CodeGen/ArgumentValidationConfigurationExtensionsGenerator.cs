namespace CodeGen;

using Microsoft.CodeAnalysis;

[Generator]
public class ArgumentValidationConfigurationExtensionsGenerator : StronglyTypedOverloadsGenerator
{
    protected override bool IsApplicable(GeneratorExecutionContext context)
    {
        return IsTypeInTargetAssembly(context, "FakeItEasy.ArgumentValidationConfigurationExtensions");
    }

    protected override string GenerateSource()
    {
        return $$"""
            namespace FakeItEasy;

            using System;
            using System.Reflection;
            using FakeItEasy.Configuration;

            public static partial class ArgumentValidationConfigurationExtensions
            {
            {{Indent(1, OverloadMethod(GenerateMethodImplementation))}}
            }
            """;
    }

    private static string GenerateMethodImplementation(int typeParametersCount)
    {
        var typeParamList = TypeParamList(typeParametersCount);
        return $$"""
            /// <summary>
            /// Configures the call to be accepted when the specified predicate returns true.
            /// </summary>
            /// <typeparam name="TInterface">The type of the interface.</typeparam>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <param name="configuration">The configuration.</param>
            /// <param name="argumentsPredicate">The argument predicate.</param>
            /// <returns>The configuration object.</returns>
            public static TInterface WhenArgumentsMatch<TInterface, {{typeParamList}}>(
                this IArgumentValidationConfiguration<TInterface> configuration,
                Func<{{typeParamList}}, bool> argumentsPredicate)
            {
                Guard.AgainstNull(configuration);
                Guard.AgainstNull(argumentsPredicate);

                return configuration.WhenArgumentsMatch(args =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(
                        args.Method,
                        argumentsPredicate.GetMethodInfo(),
                        NameOfWhenArgumentsMatchFeature);

                    return argumentsPredicate({{ArgumentsFromArgumentCollection(typeParametersCount)}});
                });
            }
            """;
    }

    private static string ArgumentsFromArgumentCollection(int typeParametersCount)
    {
        return Escalate(typeParametersCount, ", ", i => $"args.Get<T{i}>({i - 1})");
    }
}
