namespace FakeItEasy.Creation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Core;

internal class FailedCreationResult : CreationResult
{
    private readonly Type type;
    private readonly IList<string>? reasonsForFailure;
    private readonly IList<ResolvedConstructor>? consideredConstructors;
    private readonly CreationMode creationMode;

    private FailedCreationResult(
        Type type,
        CreationMode creationMode,
        IList<string>? reasonsForFailure = null,
        IList<ResolvedConstructor>? consideredConstructors = null)
    {
        this.type = type;
        this.creationMode = creationMode;
        this.reasonsForFailure = reasonsForFailure;
        this.consideredConstructors = consideredConstructors;
    }

    public override bool WasSuccessful => false;

    public override object Result =>
        throw this.creationMode.CreateException(this.GetFailedToCreateResultMessage());

    public static CreationResult ForDummy(Type type, string reasonForFailure) =>
        new FailedCreationResult(type, CreationMode.Dummy, reasonsForFailure: new List<string> { reasonForFailure });

    public static CreationResult ForDummy(Type type, List<ResolvedConstructor> consideredConstructors) =>
        new FailedCreationResult(type, CreationMode.Dummy, consideredConstructors: consideredConstructors);

    public static CreationResult ForFake(Type type, string reasonForFailure) =>
        new FailedCreationResult(type, CreationMode.Fake, reasonsForFailure: new List<string> { reasonForFailure });

    public static CreationResult ForFake(Type type, List<ResolvedConstructor> consideredConstructors) =>
        new FailedCreationResult(type, CreationMode.Fake, consideredConstructors: consideredConstructors);

    /// <summary>
    /// Returns a failed creation result combining multiple results.
    /// Reasons for failure aggregated.
    /// </summary>
    /// <param name="results">The individual failure results to merge.</param>
    /// <returns>A combined failed creation result.</returns>
    public static FailedCreationResult Merge(IList<FailedCreationResult> results)
    {
        return new FailedCreationResult(
            results[0].type,
            results[0].creationMode,
            MergeReasonsForFailure(results),
            MergeConsideredConstructors(results));
    }

    private static List<string> MergeReasonsForFailure(IEnumerable<FailedCreationResult> results) =>
        results.Select(result => result.reasonsForFailure)
            .Where(reasons => reasons is not null)
            .SelectMany(reasons => reasons!)
            .ToList();

    private static IList<ResolvedConstructor>? MergeConsideredConstructors(IList<FailedCreationResult> results) =>
        results.Select(result => result.consideredConstructors)
            .FirstOrDefault(constructors => constructors is not null);

    private string GetFailedToCreateResultMessage()
    {
        var message = new StringBuilder();

        message
            .AppendLine()
            .AppendIndented("  ", $"Failed to create {this.creationMode.Name} of type ")
            .Append(this.type)
            .Append(':');

        if (this.reasonsForFailure is not null)
        {
            foreach (var reasonForFailure in this.reasonsForFailure)
            {
                message
                    .AppendLine()
                    .AppendIndented("    ", reasonForFailure);
            }
        }

        message
            .AppendLine();

        if (this.consideredConstructors is not null && this.consideredConstructors.Any(x => x.WasSuccessfullyResolved))
        {
            message
                .AppendLine()
                .AppendIndented("  ", "Below is a list of reasons for failure per attempted constructor:");

            foreach (var constructor in this.consideredConstructors.Where(x => x.WasSuccessfullyResolved))
            {
                message
                    .AppendLine()
                    .AppendIndented("    ", "Constructor with signature (")
                    .Append(constructor.Arguments.ToCollectionString(x => x.ArgumentType.ToString(), ", "))
                    .AppendLine(") failed:")
                    .AppendIndented("      ", constructor.ReasonForFailure!)
                    .AppendLine();
            }
        }

        this.AppendNonTriedConstructors(message);
        return message.ToString();
    }

    private void AppendNonTriedConstructors(StringBuilder message)
    {
        if (this.consideredConstructors is null || this.consideredConstructors.All(x => x.WasSuccessfullyResolved))
        {
            return;
        }

        message
            .AppendLine()
            .AppendIndented("  ", "The constructors with the following signatures were not tried:")
            .AppendLine();

        foreach (var resolvedConstructor in this.consideredConstructors.Where(x => !x.WasSuccessfullyResolved))
        {
            message
                .Append("    (")
                .Append(resolvedConstructor.Arguments.ToCollectionString(x => x.WasResolved ? x.ArgumentType.ToString() : string.Concat("*", x.ArgumentType.ToString()), ", "))
                .AppendLine(")");
        }

        message
            .AppendLine()
            .AppendIndented("    ", "Types marked with * could not be resolved. Please provide a Dummy Factory to enable these constructors.")
            .AppendLine()
            .AppendLine();
    }

    private sealed class CreationMode
    {
        private readonly Func<string, Exception> exceptionFactory;

        private CreationMode(string name, Func<string, Exception> exceptionFactory)
        {
            this.Name = name;
            this.exceptionFactory = exceptionFactory;
        }

        public static CreationMode Fake { get; } = new CreationMode("fake", message => new FakeCreationException(message));

        public static CreationMode Dummy { get; } = new CreationMode("dummy", message => new DummyCreationException(message));

        public string Name { get; }

        public Exception CreateException(string message) => this.exceptionFactory(message);
    }
}
