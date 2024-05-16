namespace CodeGen;

using Microsoft.CodeAnalysis;

[Generator]
public class ValueTaskAsyncReturnValueConfigurationExtensionsGenerator : StronglyTypedOverloadsGenerator
{
    protected override bool IsApplicable(GeneratorExecutionContext context)
    {
        return IsTypeInTargetAssembly(context, "FakeItEasy.ValueTaskAsyncReturnValueConfigurationExtensions");
    }

    protected override string GenerateSource()
    {
        return $$"""
            namespace FakeItEasy
            {
                using System;
                using System.Threading.Tasks;
                using FakeItEasy.Configuration;

                public static partial class ValueTaskAsyncReturnValueConfigurationExtensions
                {
            {{Indent(2, OverloadMethod(GenerateMethodImplementation))}}
                }
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
            /// Returns a failed ValueTask with the specified exception when the currently configured call gets called.
            /// </summary>
            /// <param name="configuration">The configuration to use.</param>
            /// <param name="exceptionFactory">A function that returns the exception to set on the returned ValueTask when a call that matches is invoked.</param>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <returns>The configuration object.</returns>
            public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<ValueTask>> ThrowsAsync<{{typeParamList}}>(
                this IReturnValueConfiguration<ValueTask> configuration,
                Func<{{typeParamList}}, Exception> exceptionFactory)
            {
                Guard.AgainstNull(configuration);
                Guard.AgainstNull(exceptionFactory);

                return
                    configuration.ReturnsLazily(
                        ({{lambdaParamList}}) => new ValueTask(TaskHelper.FromException(exceptionFactory({{argumentList}}))));
            }

            /// <summary>
            /// Returns a failed ValueTask with the specified exception when the currently configured call gets called.
            /// </summary>
            /// <param name="configuration">The configuration to use.</param>
            /// <param name="exceptionFactory">A function that returns the exception to set on the returned ValueTask when a call that matches is invoked.</param>
            /// <typeparam name="T">The type of the returned ValueTask's result.</typeparam>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <returns>The configuration object.</returns>
            public static IAfterCallConfiguredConfiguration<IReturnValueConfiguration<ValueTask<T>>> ThrowsAsync<T, {{typeParamList}}>(
                this IReturnValueConfiguration<ValueTask<T>> configuration,
                Func<{{typeParamList}}, Exception> exceptionFactory)
            {
                Guard.AgainstNull(configuration);
                Guard.AgainstNull(exceptionFactory);

                return configuration.ReturnsLazily(({{lambdaParamList}}) => new ValueTask<T>(TaskHelper.FromException<T>(exceptionFactory({{argumentList}}))));
            }
            """;
    }
}
