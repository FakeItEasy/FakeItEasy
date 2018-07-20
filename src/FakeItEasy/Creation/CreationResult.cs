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
        private readonly string reasonForFailure;
        private readonly IList<ResolvedConstructor> consideredConstructors;
        private readonly object result;

        private CreationResult(object result)
        {
            this.WasSuccessful = true;
            this.result = result;
        }

        private CreationResult(
            Type type,
            string reasonForFailure,
            IList<ResolvedConstructor> consideredConstructors)
        {
            this.WasSuccessful = false;
            this.type = type;
            this.reasonForFailure = reasonForFailure;
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

                throw new FakeCreationException(this.GetFailedToCreateResultMessage());
            }
        }

        public static CreationResult SuccessfullyCreated(object result)
        {
            return new CreationResult(result);
        }

        public static CreationResult FailedToCreate(Type type, string reasonForFailure)
        {
            return new CreationResult(type, reasonForFailure, null);
        }

        public static CreationResult FailedToCreate(Type type, List<ResolvedConstructor> consideredConstructors)
        {
            return new CreationResult(type, null, consideredConstructors);
        }

        private string GetFailedToCreateResultMessage()
        {
            var message = new StringBuilder();

            message
                .AppendLine()
                .AppendIndented("  ", "Failed to create fake of type ")
                .Append(this.type)
                .AppendLine(":");

            if (this.reasonForFailure != null)
            {
                message
                    .AppendIndented("    ", this.reasonForFailure);
            }

            message
                .AppendLine();

            if (this.consideredConstructors != null && this.consideredConstructors.Any())
            {
                message
                    .AppendIndented("  ", "Below is a list of reasons for failure per attempted constructor:")
                    .AppendLine();

                if (this.consideredConstructors.Any(x => x.WasSuccessfullyResolved))
                {
                    foreach (var constructor in this.consideredConstructors.Where(x => x.WasSuccessfullyResolved))
                    {
                        message
                            .AppendIndented("    ", "Constructor with signature (")
                            .Append(constructor.Arguments.ToCollectionString(x => x.ArgumentType.ToString(), ", "))
                            .AppendLine(") failed:")
                            .AppendIndented("      ", constructor.ReasonForFailure)
                            .AppendLine();
                    }
                }

                this.AppendNonTriedConstructors(message);
                message
                    .AppendLine();
            }

            return message.ToString();
        }

        private void AppendNonTriedConstructors(StringBuilder message)
        {
            if (this.consideredConstructors.Any(x => !x.WasSuccessfullyResolved))
            {
                message
                    .AppendIndented("    ", "The following constructors were not tried:")
                    .AppendLine();

                foreach (var resolvedConstructor in this.consideredConstructors.Where(x => !x.WasSuccessfullyResolved))
                {
                    message.Append("      (");
                    message.Append(resolvedConstructor.Arguments.ToCollectionString(x => x.WasResolved ? x.ArgumentType.ToString() : String.Concat("*", x.ArgumentType.ToString()), ", "));
                    message.AppendLine(")");
                }

                message
                    .AppendLine()
                    .AppendIndented("      ", "Types marked with * could not be resolved. Please provide a Dummy Factory to enable these constructors.")
                    .AppendLine();
            }
        }
    }
}
