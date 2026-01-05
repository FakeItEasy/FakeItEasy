namespace CodeGen;

using Microsoft.CodeAnalysis;

[Generator]
public class AsyncReturnValueConfigurationExtensionsGenerator : StronglyTypedOverloadsGenerator
{
    protected override bool IsApplicable(GeneratorExecutionContext context)
    {
        return IsTypeInTargetAssembly(context, "FakeItEasy.AsyncReturnValueConfigurationExtensions");
    }

    protected override string GenerateSource()
    {
        return $$"""
            namespace FakeItEasy;

            using System;
            using System.Threading.Tasks;
            using FakeItEasy.Configuration;

            public static partial class AsyncReturnValueConfigurationExtensions
            {
            {{Indent(1, OverloadMethod(GenerateMethodImplementation))}}
            }
            """;
    }

    private static string GenerateMethodImplementation(int typeParametersCount)
    {
        var typeParamList = TypeParamList(typeParametersCount);
        var lambdaParamList = Escalate(typeParametersCount, ", ", i => $"T{i} arg{i}");
        var argumentList = Escalate(typeParametersCount, ", ", i => $"arg{i}");

        return $$"""
            /// <summary>
            /// Returns a failed task with the specified exception when the currently configured call gets called.
            /// </summary>
            /// <param name="configuration">The configuration to use.</param>
            /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <returns>The configuration object.</returns>
            public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task>> ThrowsAsync<{{typeParamList}}>(
                this IReturnValueConfiguration<Task> configuration,
                Func<{{typeParamList}}, Exception> exceptionFactory)
            {
                Guard.AgainstNull(configuration);
                Guard.AgainstNull(exceptionFactory);

                return
                    configuration.ReturnsLazily(
                        ({{lambdaParamList}}) => TaskHelper.FromException(exceptionFactory({{argumentList}})));
            }

            /// <summary>
            /// Returns a failed task with the specified exception when the currently configured call gets called.
            /// </summary>
            /// <param name="configuration">The configuration to use.</param>
            /// <param name="exceptionFactory">A function that returns the exception to set on the returned task when a call that matches is invoked.</param>
            /// <typeparam name="T">The type of the returned task's result.</typeparam>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <returns>The configuration object.</returns>
            public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<Task<T>>> ThrowsAsync<T, {{typeParamList}}>(
                this IReturnValueConfiguration<Task<T>> configuration,
                Func<{{typeParamList}}, Exception> exceptionFactory)
            {
                Guard.AgainstNull(configuration);
                Guard.AgainstNull(exceptionFactory);

                return configuration.ReturnsLazily(({{lambdaParamList}}) => TaskHelper.FromException<T>(exceptionFactory({{argumentList}})));
            }
            """;
    }
}
