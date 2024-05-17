namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy.Core;

    internal abstract class CreationResult(int status)
    {
        private const int SuccessfulCreation = 0;
        private const int InterimFailedCreation = 1;
        private const int PermanentlyFailedCreation = 2;

        private readonly int status = status;

        public bool WasSuccessful => this.status == SuccessfulCreation;

        public bool IsFinal => this.status != InterimFailedCreation;

        public abstract object? Result { get; }

        public static CreationResult SuccessfullyCreated(object? result) =>
             new SuccessfulCreationResult(result);

        public static CreationResult FailedToCreateDummy(Type type, string reasonForFailure) =>
            new FailedCreationResult(InterimFailedCreation, type, CreationMode.Dummy, reasonsForFailure: new List<string> { reasonForFailure });

        public static CreationResult FailedToCreateDummy(Type type, List<ResolvedConstructor> consideredConstructors) =>
            new FailedCreationResult(InterimFailedCreation, type, CreationMode.Dummy, consideredConstructors: consideredConstructors);

        public static CreationResult FailedToCreateFake(Type type, string reasonForFailure) =>
            new FailedCreationResult(InterimFailedCreation, type, CreationMode.Fake, reasonsForFailure: new List<string> { reasonForFailure });

        public static CreationResult FailedToCreateFake(Type type, List<ResolvedConstructor> consideredConstructors) =>
            new FailedCreationResult(InterimFailedCreation, type, CreationMode.Fake, consideredConstructors: consideredConstructors);

        public static CreationResult PermanentlyFailedToCreateDummy(Type type, string reasonForFailure) =>
            new FailedCreationResult(PermanentlyFailedCreation, type, CreationMode.Dummy, reasonsForFailure: new List<string> { reasonForFailure });

        /// <summary>
        /// Returns a creation result for a dummy by combining two results.
        /// Successful results are preferred to failed. Failed results will have their reasons for failure aggregated.
        /// </summary>
        /// <param name="previousResult">May be <c>null</c> if <paramref name="newResult"/> is the result of the first attempt to create the dummy.</param>
        /// <param name="newResult">The other result to merge. Must not be <c>null</c>.</param>
        /// <returns>A combined creation result. Successful if either <c>newResult</c> was successful, and failed otherwise.</returns>
        public static CreationResult MergeResults(CreationResult? previousResult, CreationResult newResult)
        {
            Guard.AgainstNull(newResult);
            return previousResult is null || newResult is not FailedCreationResult newFailedResult
                ? newResult
                : newFailedResult.MergeIntoDummyResult(previousResult);
        }

        private class SuccessfulCreationResult(object? result) : CreationResult(SuccessfulCreation)
        {
            public override object? Result { get; } = result;
        }

        private class FailedCreationResult(
            int status,
            Type type,
            CreationMode creationMode,
            IList<string>? reasonsForFailure = null,
            IList<ResolvedConstructor>? consideredConstructors = null)
            : CreationResult(status)
        {
            private readonly IList<string>? reasonsForFailure = reasonsForFailure;
            private readonly IList<ResolvedConstructor>? consideredConstructors = consideredConstructors;

            public override object Result =>
                    throw creationMode.CreateException(this.BuildFailedToCreateResultMessage());

            public FailedCreationResult MergeIntoDummyResult(CreationResult other)
            {
                Guard.AgainstNull(other);

                var failedOther = (FailedCreationResult)other;
                return new FailedCreationResult(
                    this.status,
                    type,
                    creationMode,
                    MergeReasonsForFailure(failedOther.reasonsForFailure, this.reasonsForFailure),
                    MergeConsideredConstructors(failedOther.consideredConstructors, this.consideredConstructors));
            }

            private static IList<string>? MergeReasonsForFailure(
                IList<string>? reasonsFromResult1,
                IList<string>? reasonsFromResult2)
            {
                if (reasonsFromResult1 is null)
                {
                    return reasonsFromResult2;
                }

                if (reasonsFromResult2 is null)
                {
                    return reasonsFromResult1;
                }

                var mergedList = new List<string>(reasonsFromResult1);
                mergedList.AddRange(reasonsFromResult2);
                return mergedList;
            }

            private static IList<ResolvedConstructor>? MergeConsideredConstructors(
                IList<ResolvedConstructor>? constructorsFromResult1,
                IList<ResolvedConstructor>? constructorsFromResult2)
            {
                if (constructorsFromResult1 is null)
                {
                    return constructorsFromResult2;
                }

                if (constructorsFromResult2 is null)
                {
                    return constructorsFromResult1;
                }

                return constructorsFromResult1.Union(constructorsFromResult2, ResolvedConstructorComparer.Default).ToList();
            }

            private string BuildFailedToCreateResultMessage()
            {
                var message = new StringBuilder();

                message
                    .AppendLine()
                    .AppendIndented("  ", $"Failed to create {creationMode.Name} of type ")
                    .Append(type)
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
        }

        private class ResolvedConstructorComparer : IEqualityComparer<ResolvedConstructor>
        {
            private ResolvedConstructorComparer()
            {
            }

            public static ResolvedConstructorComparer Default { get; } = new ResolvedConstructorComparer();

            public bool Equals(ResolvedConstructor? x, ResolvedConstructor? y)
            {
                return ReferenceEquals(x, y) || (x is not null && y is not null && ConstructorArgumentTypes(x).SequenceEqual(ConstructorArgumentTypes(y)));

                IEnumerable<Type> ConstructorArgumentTypes(ResolvedConstructor constructor) =>
                    constructor.Arguments.Select(a => a.ArgumentType);
            }

            public int GetHashCode(ResolvedConstructor obj)
            {
                unchecked
                {
                    var hash = 17;

                    foreach (var resolvedArgument in obj.Arguments)
                    {
                        hash = (hash * 23) + EqualityComparer<object>.Default.GetHashCode(resolvedArgument.ArgumentType);
                    }

                    return hash;
                }
            }
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
}
