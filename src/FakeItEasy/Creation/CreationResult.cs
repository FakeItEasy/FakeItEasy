namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy.Core;

    internal sealed class CreationResult
    {
        private readonly Type type;
        private readonly IList<string> reasonsForFailure;
        private readonly IList<ResolvedConstructor> consideredConstructors;
        private readonly object result;

        private CreationMode creationMode;

        private CreationResult(object result)
        {
            this.WasSuccessful = true;
            this.result = result;
        }

        private CreationResult(
            Type type,
            CreationMode creationMode,
            IList<string> reasonsForFailure = null,
            IList<ResolvedConstructor> consideredConstructors = null)
        {
            this.WasSuccessful = false;
            this.type = type;
            this.creationMode = creationMode;
            this.reasonsForFailure = reasonsForFailure;
            this.consideredConstructors = consideredConstructors;
        }

        public bool WasSuccessful { get; }

        public object Result
        {
            get
            {
                if (this.WasSuccessful)
                {
                    return this.result;
                }

                var message = this.GetFailedToCreateResultMessage();
                throw this.creationMode.CreateException(message);
            }
        }

        public static CreationResult SuccessfullyCreated(object result)
        {
            return new CreationResult(result);
        }

        public static CreationResult FailedToCreateDummy(Type type, string reasonForFailure) =>
            new CreationResult(type, CreationMode.Dummy, reasonsForFailure: new List<string> { reasonForFailure });

        public static CreationResult FailedToCreateDummy(Type type, List<ResolvedConstructor> consideredConstructors) =>
            new CreationResult(type, CreationMode.Dummy, consideredConstructors: consideredConstructors);

        public static CreationResult FailedToCreateFake(Type type, string reasonForFailure) =>
            new CreationResult(type, CreationMode.Fake, reasonsForFailure: new List<string> { reasonForFailure });

        public static CreationResult FailedToCreateFake(Type type, List<ResolvedConstructor> consideredConstructors) =>
            new CreationResult(type, CreationMode.Fake, consideredConstructors: consideredConstructors);

        /// <summary>
        /// Returns a creation result for a dummy by combining two results.
        /// Successful results are preferred to failed. Failed results will have their reasons for failure aggregated.
        /// </summary>
        /// <param name="one">One result to merge. May be <code>null</code>.</param>
        /// <param name="other">The other result to merge. Must not be <code>null</code>.</param>
        /// <returns>A combined creation result. Successful if either input was successful, and failed otherwise.</returns>
        public static CreationResult MergeIntoDummyResult(CreationResult one, CreationResult other)
        {
            Guard.AgainstNull(other, "other");

            if (one == null || other.WasSuccessful)
            {
                return other.AsDummyResult();
            }

            if (one.WasSuccessful)
            {
                return one.AsDummyResult();
            }

            return new CreationResult(
                one.type,
                CreationMode.Dummy,
                MergeReasonsForFailure(one.reasonsForFailure, other.reasonsForFailure),
                MergeConsideredConstructors(one.consideredConstructors, other.consideredConstructors));
        }

        private static IList<string> MergeReasonsForFailure(
            IList<string> reasonsFromResult1,
            IList<string> reasonsFromResult2)
        {
            if (reasonsFromResult1 == null)
            {
                return reasonsFromResult2;
            }

            if (reasonsFromResult2 == null)
            {
                return reasonsFromResult1;
            }

            var mergedList = new List<string>(reasonsFromResult1);
            mergedList.AddRange(reasonsFromResult2);
            return mergedList;
        }

        private static IList<ResolvedConstructor> MergeConsideredConstructors(
            IList<ResolvedConstructor> constructorsFromResult1,
            IList<ResolvedConstructor> constructorsFromResult2)
        {
            if (constructorsFromResult1 == null)
            {
                return constructorsFromResult2;
            }

            if (constructorsFromResult2 == null)
            {
                return constructorsFromResult1;
            }

            return constructorsFromResult1.Union(constructorsFromResult2, ResolvedConstructorComparer.Default).ToList();
        }

        private CreationResult AsDummyResult()
        {
            return this.creationMode == CreationMode.Dummy
                ? this
                : new CreationResult(this.type, CreationMode.Dummy, this.reasonsForFailure, this.consideredConstructors);
        }

        private string GetFailedToCreateResultMessage()
        {
            var message = new StringBuilder();

            message
                .AppendLine()
                .AppendIndented("  ", $"Failed to create {this.creationMode.Name} of type ")
                .Append(this.type)
                .Append(":");

            if (this.reasonsForFailure != null)
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

            if (this.consideredConstructors != null && this.consideredConstructors.Any(x => x.WasSuccessfullyResolved))
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
                        .AppendIndented("      ", constructor.ReasonForFailure)
                        .AppendLine();
                }
            }

            this.AppendNonTriedConstructors(message);
            return message.ToString();
        }

        private void AppendNonTriedConstructors(StringBuilder message)
        {
            if (this.consideredConstructors == null || this.consideredConstructors.All(x => x.WasSuccessfullyResolved))
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
                    .Append(resolvedConstructor.Arguments.ToCollectionString(x => x.WasResolved ? x.ArgumentType.ToString() : String.Concat("*", x.ArgumentType.ToString()), ", "))
                    .AppendLine(")");
            }

            message
                .AppendLine()
                .AppendIndented("    ", "Types marked with * could not be resolved. Please provide a Dummy Factory to enable these constructors.")
                .AppendLine()
                .AppendLine();
        }

        private class ResolvedConstructorComparer : IEqualityComparer<ResolvedConstructor>
        {
            private ResolvedConstructorComparer()
            {
            }

            public static ResolvedConstructorComparer Default { get; } = new ResolvedConstructorComparer();

            public bool Equals(ResolvedConstructor x, ResolvedConstructor y)
            {
                return ReferenceEquals(x, y) || (x != null && y != null && ConstructorArgumentTypes(x).SequenceEqual(ConstructorArgumentTypes(y)));

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
