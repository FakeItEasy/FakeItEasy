namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy.Core;

    internal abstract class CreationResult
    {
        public static CreationResult Untried { get; } = new UntriedCreationResult();

        public abstract bool WasSuccessful { get; }

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

        private class UntriedCreationResult : CreationResult
        {
            public override bool WasSuccessful => false;

            public override object Result => throw new NotSupportedException();

            public override CreationResult MergeIntoDummyResult(CreationResult other) => other;
        }

        private class SuccessfulCreationResult : CreationResult
        {
            public SuccessfulCreationResult(object? result) => this.Result = result;

            public override bool WasSuccessful => true;

            public override object? Result { get; }

            public override CreationResult MergeIntoDummyResult(CreationResult other) => this;
        }

        private class FailedCreationResult : CreationResult
        {
            private readonly Type type;
            private readonly IList<string>? reasonsForFailure;
            private readonly IList<ResolvedConstructor>? consideredConstructors;
            private CreationMode creationMode;

            public FailedCreationResult(
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

            public override object? Result =>
                    throw this.creationMode.CreateException(this.GetFailedToCreateResultMessage());

            public override CreationResult MergeIntoDummyResult(CreationResult other)
            {
                Guard.AgainstNull(other);

                if (other.WasSuccessful)
                {
                    return other;
                }

                var failedOther = (FailedCreationResult)other;
                return new FailedCreationResult(
                    this.type,
                    CreationMode.Dummy,
                    MergeReasonsForFailure(this.reasonsForFailure, failedOther.reasonsForFailure),
                    this.consideredConstructors ?? failedOther.consideredConstructors);
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
