namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Xbehave;

    /// <summary>
    /// Check that certain fluent API syntax constructions are illegal.
    /// </summary>
    public static class IllegalFluentApiSpecs
    {
        private const string FakeUsageTemplate = @"
namespace ApiTest
{{
    using FakeItEasy;
    public class Test
    {{
        public interface IService
        {{
            void AVoidMethod();

            int AnIntMethod();

            int AProperty {{ get; set; }}
        }}

        public void TestMethod()
        {{
            var fake = A.Fake<IService>();
            {0};
        }}
    }}
}}
";

        private static IEnumerable<Microsoft.CodeAnalysis.MetadataReference> references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Where(CompilationRequiresAssembly)
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        [Scenario]
        [UsingCulture("en-US")]
        [Example(
            "A.CallTo(fake).Returns(3)",
            "does not contain a definition for 'Returns'")]
        [Example(
            "A.CallTo(fake).WithReturnType<string>().Returns(3)",
            "does not contain a definition for 'Returns'")]
        [Example(
            "A.CallTo(fake).Invokes(() => {}).MustHaveHappened(4, Times.OrLess)",
            "No overload for method 'MustHaveHappened'")]
        [Example(
            "A.CallTo(fake).Invokes(() => {}).MustHaveHappenedANumberOfTimesMatching(count => true)",
            "does not contain a definition for 'MustHaveHappenedANumberOfTimesMatching'")]
        [Example(
            "A.CallTo(fake).CallsWrappedMethod().MustHaveHappened(4, Times.OrLess)",
            "No overload for method 'MustHaveHappened'")]
        [Example(
            "A.CallTo(fake).CallsWrappedMethod().MustHaveHappenedANumberOfTimesMatching(count => true)",
            "does not contain a definition for 'MustHaveHappenedANumberOfTimesMatching'")]
        [Example(
            "A.CallTo(fake).WithNonVoidReturnType().Returns(0).MustHaveHappened(4, Times.OrLess)",
            "No overload for method 'MustHaveHappened'")]
        [Example(
            "A.CallTo(fake).WithNonVoidReturnType().Returns(0).MustHaveHappenedANumberOfTimesMatching(count => true)",
            "does not contain a definition for 'MustHaveHappenedANumberOfTimesMatching'")]
        [Example(
            "A.CallTo(fake).WhenArgumentsMatch(call => true).WithNonVoidReturnType()",
            "does not contain a definition for 'WithNonVoidReturnType'")]
        [Example(
            "A.CallTo(() => fake.AVoidMethod()).WithNonVoidReturnType()",
            "does not contain a definition for 'WithNonVoidReturnType'")]
        [Example(
            "A.CallTo(() => fake.AVoidMethod()).WithReturnType<string>()",
            "does not contain a definition for 'WithReturnType'")]
        [Example(
            "A.CallTo(() => fake.AVoidMethod()).Returns(3)",
            "does not contain a definition for 'Returns'")]
        [Example(
            "A.CallTo(() => fake.AnIntMethod()).WithNonVoidReturnType()",
            "does not contain a definition for 'WithNonVoidReturnType'")]
        [Example(
            "A.CallTo(() => fake.AnIntMethod()).WithReturnType<int>()",
            "does not contain a definition for 'WithReturnType'")]
        [Example(
            "A.CallTo(() => fake.AnIntMethod()).Returns(new object())",
            "does not contain a definition for 'Returns'")]
        [Example(
            "A.CallTo(() => fake.AnIntMethod()).Returns(3).Invokes(() => {})",
            "does not contain a definition for 'Invokes'")]
        [Example(
            "A.CallTo(() => fake.AnIntMethod()).Throws<System.Exception>().MustHaveHappened()",
            "does not contain a definition for 'MustHaveHappened'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).Returns(3)",
            "does not contain a definition for 'Returns'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).SetsOutAndRefParameters('a')",
            "does not contain a definition for 'SetsOutAndRefParameters'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).WithReturnType<string>()",
            "does not contain a definition for 'WithReturnType'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).WithNonVoidReturnType()",
            "does not contain a definition for 'WithNonVoidReturnType'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).Invokes(() => {}).MustHaveHappened()",
            "does not contain a definition for 'MustHaveHappened'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).Throws<System.Exception>().MustHaveHappened()",
            "does not contain a definition for 'MustHaveHappened'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).To(7).Returns(3)",
            "does not contain a definition for 'Returns'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).To(7).SetsOutAndRefParameters('a')",
            "does not contain a definition for 'SetsOutAndRefParameters'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).To(7).WithReturnType<string>()",
            "does not contain a definition for 'WithReturnType'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).To(7).WithNonVoidReturnType()",
            "does not contain a definition for 'WithNonVoidReturnType'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).To(7).Invokes(() => {}).MustHaveHappened()",
            "does not contain a definition for 'MustHaveHappened'")]
        [Example(
            "A.CallToSet(() => fake.AProperty).To(7).Throws<System.Exception>().MustHaveHappened()",
            "does not contain a definition for 'MustHaveHappened'")]
        public static void IllegalSyntax(string illegalOperation, string expectedError, string illegalUsage, IEnumerable<Diagnostic> diagnostics)
        {
            "Given an illegal usage of a fake"
                .x(() => illegalUsage = string.Format(CultureInfo.InvariantCulture, FakeUsageTemplate, illegalOperation));

            "When I compile the usage"
                .x(() => diagnostics = Compile(illegalUsage));

            "Then the compilation fails"
                .x(() => diagnostics.Should().ContainSingle().Which.GetMessage().Should()
                    .Contain(expectedError));
        }

        private static bool CompilationRequiresAssembly(Assembly a)
        {
            var assemblyName = a.GetName().Name;
            return
                assemblyName == "mscorlib" ||
                assemblyName == "netstandard" ||
                assemblyName == "System.Core" ||
                assemblyName == "System.Linq.Expressions" ||
                assemblyName == "System.Private.CoreLib" ||
                assemblyName == "System.Runtime" ||
                assemblyName == "FakeItEasy";
        }

        private static System.Collections.Immutable.ImmutableArray<Diagnostic> Compile(string source)
        {
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithOptimizationLevel(OptimizationLevel.Release);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(
                source,
                CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest),
                string.Empty);

            var compilation = CSharpCompilation.Create(
                "Test.dll",
                new[] { parsedSyntaxTree },
                references,
                compilationOptions);
            return compilation.GetDiagnostics();
        }
    }
}
