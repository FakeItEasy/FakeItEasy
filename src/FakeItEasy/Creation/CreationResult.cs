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
        private const int FailedCreation = 1;

        public static CreationResult Untried { get; } = new UntriedCreationResult();

        public bool WasSuccessful => status == SuccessfulCreation;

        public abstract object? Result { get; }

        public static CreationResult SuccessfullyCreated(object? result) =>
             new SuccessfulCreationResult(result);

        public static CreationResult FailedToCreateDummy(Type type, string reasonForFailure) =>
            new FailedCreationResult(type, CreationMode.Dummy, reasonsForFailure: new List<string> { reasonForFailure });

        public static CreationResult FailedToCreateDummy(Type type, List<ResolvedConstructor> consideredConstructors) =>
            new FailedCreationResult(type, CreationMode.Dummy, consideredConstructors: consideredConstructors);

        public static CreationResult FailedToCreateFake(Type type, string reasonForFailure) =>
            new FailedCreationResult(type, CreationMode.Fake, reasonsForFailure: new List<string> { reasonForFailure });

        public static CreationResult FailedToCreateFake(Type type, List<ResolvedConstructor> consideredConstructors) =>
            new FailedCreationResult(type, CreationMode.Fake, consideredConstructors: consideredConstructors);

        /// <summary>
        /// Returns a creation result for a dummy by combining two results.
        /// Successful results are preferred to failed. Failed results will have their reasons for failure aggregated.
        /// </summary>
        /// <param name="other">The other result to merge. Must not be <c>null</c>.</param>
        /// <returns>A combined creation result. Successful if either input was successful, and failed otherwise.</returns>
        public abstract CreationResult MergeIntoDummyResult(CreationResult other);

        private class UntriedCreationResult() : CreationResult(FailedCreation)
        {
            public override object Result => throw new NotSupportedException();

            public override CreationResult MergeIntoDummyResult(CreationResult other) => other;
        }

        private class SuccessfulCreationResult(object? result) : CreationResult(SuccessfulCreation)
        {
            public override object? Result { get; } = result;

            public override CreationResult MergeIntoDummyResult(CreationResult other) => this;
        }

        private class FailedCreationResult(
            Type type,
            CreationMode creationMode,
            IList<string>? reasonsForFailure = null,
            IList<ResolvedConstructor>? consideredConstructors = null)
            : CreationResult(FailedCreation)
        {
            private readonly IList<string>? reasonsForFailure = reasonsForFailure;
            private readonly IList<ResolvedConstructor>? consideredConstructors = consideredConstructors;

            public override object Result =>
                    throw creationMode.CreateException(this.BuildFailedToCreateResultMessage());

            public override CreationResult MergeIntoDummyResult(CreationResult other)
            {
                Guard.AgainstNull(other);

                if (other.WasSuccessful)
                {
                    return other;
                }

                var failedOther = (FailedCreationResult)other;
                return new FailedCreationResult(
                    type,
                    CreationMode.Dummy,
                    MergeReasonsForFailure(this.reasonsForFailure, failedOther.reasonsForFailure),
                    MergeConsideredConstructors(this.consideredConstructors, failedOther.consideredConstructors));
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
