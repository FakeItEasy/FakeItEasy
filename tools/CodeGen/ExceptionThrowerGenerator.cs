namespace CodeGen;

using Microsoft.CodeAnalysis;

[Generator]
public class ExceptionThrowerGenerator : StronglyTypedOverloadsGenerator
{
    protected override bool IsApplicable(GeneratorExecutionContext context)
    {
        return IsTypeInTargetAssembly(context, "FakeItEasy.Configuration.IExceptionThrowerConfiguration`1");
    }

    protected override string GenerateSource()
    {
        return $$"""
            namespace FakeItEasy.Configuration;

            using System;
            using System.Reflection;
            using FakeItEasy.Core;

            public partial interface IExceptionThrowerConfiguration<out TInterface>
            {
            {{Indent(1, OverloadMethod(GenerateMethodDeclaration))}}
            }

            internal partial class RuleBuilder
            {
            {{Indent(1, GenerateMethodImplementations("IVoidConfiguration"))}}

                public partial class ReturnValueConfiguration<TMember>
                {
            {{Indent(2, GenerateMethodImplementations("IReturnValueConfiguration<TMember>"))}}
                }
            }

            internal partial class AnyCallConfiguration
            {
            {{Indent(1, GenerateMethodImplementations("IVoidConfiguration"))}}
            }

            internal partial class PropertySetterConfiguration<TValue>
            {
            {{Indent(1, GenerateMethodImplementations("IPropertySetterConfiguration"))}}

                private partial class PropertySetterAdapter
                {
            {{Indent(2, GenerateMethodImplementations("IPropertySetterConfiguration"))}}
                }

                private partial class PropertySetterAfterCallbackConfiguredAdapter
                {
            {{Indent(2, GenerateMethodImplementations("IPropertySetterConfiguration"))}}
                }
            }
            """;
    }

    private static string GenerateMethodDeclaration(int typeParametersCount)
    {
        var typeParamList = TypeParamList(typeParametersCount);
        return $"""
            /// <summary>
            /// Throws the specified exception when the currently configured
            /// call gets called.
            /// </summary>
            /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
            {TypeParamDocsSource(typeParametersCount)}
            /// <returns>Configuration object.</returns>
            /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
            IAfterCallConfiguredConfiguration<TInterface> Throws<{typeParamList}>(Func<{typeParamList}, Exception> exceptionFactory);
            """;
    }

    private static string GenerateMethodImplementations(string configurationInterface) =>
        OverloadMethod(i => GenerateMethodImplementation(configurationInterface, i));

    private static string GenerateMethodImplementation(string configurationInterface, int typeParametersCount)
    {
        var typeParamList = TypeParamList(typeParametersCount);
        return $$"""
            /// <summary>
            /// Throws the specified exception when the currently configured
            /// call gets called.
            /// </summary>
            /// <param name="exceptionFactory">A function that returns the exception to throw when invoked.</param>
            {{TypeParamDocsSource(typeParametersCount)}}
            /// <returns>Configuration object.</returns>
            /// <exception cref="FakeConfigurationException">The signatures of the faked method and the <paramref name="exceptionFactory"/> do not match.</exception>
            public IAfterCallConfiguredConfiguration<{{configurationInterface}}> Throws<{{typeParamList}}>(Func<{{typeParamList}}, Exception> exceptionFactory)
            {
                Guard.AgainstNull(exceptionFactory);

                return this.Throws((IFakeObjectCall call) =>
                {
                    ValueProducerSignatureHelper.AssertThatValueProducerSignatureSatisfiesCallSignature(call.Method, exceptionFactory.GetMethodInfo(), "throws");

                    return exceptionFactory({{ArgumentsFromCall(typeParametersCount)}});
                });
            }
            """;
    }
}
