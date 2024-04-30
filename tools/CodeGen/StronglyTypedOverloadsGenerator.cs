namespace CodeGen;

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

public abstract class StronglyTypedOverloadsGenerator : ISourceGenerator
{
    protected const int MaximumTypeParameterCount = 8;

    private static readonly string[] Ordinals =
    [
        "zeroth",
        "first",
        "second",
        "third",
        "fourth",
        "fifth",
        "sixth",
        "seventh",
        "eighth"
    ];

    public abstract void Execute(GeneratorExecutionContext context);

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required
    }

    protected static string TypeParamDocsSource(int typeParametersCount) =>
        Escalate(
            typeParametersCount,
            "\n",
            i => $"""/// <typeparam name="T{i}">The type of the {Ordinals[i]} argument of the faked method call.</typeparam>""");

    protected static string TypeParamList(int typeParametersCount) =>
        Escalate(typeParametersCount, ", ", TypeParamName);

    /// <summary>
    /// Generates a string by invoking the <paramref name="generator" /> function for each number
    /// from 1 to <paramref name="maximum" /> and joining the results
    /// with <paramref name="separator"/>.
    /// </summary>
    /// <param name="maximum">The maximum number to generate strings for.</param>
    /// <param name="separator">The separator to use between generated strings.</param>
    /// <param name="generator">The function to generate strings.</param>
    /// <returns>The generated string.</returns>
    protected static string Escalate(int maximum, string separator, Func<int, string> generator) =>
        string.Join(separator, Enumerable.Range(1, maximum).Select(generator));

    protected static string Indent(int depth, string text)
    {
        var prefix = new string(' ', 4 * depth);
        return string.Join(
            "\r\n",
            text.Split(["\r\n", "\n"], StringSplitOptions.None)
                .Select(line => line.Length == 0 ? line : prefix + line));
    }

    protected static string ArgumentsFromCall(int typeParametersCount)
    {
        return Escalate(typeParametersCount, ", ", i => $"call.GetArgument<T{i}>({i - 1})!");
    }

    private static string TypeParamName(int index) => $"T{index}";
}
