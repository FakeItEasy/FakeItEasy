namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy.Creation;

    internal class DefaultExceptionThrower
        : IExceptionThrower
    {
        public void ThrowFailedToGenerateProxyWithArgumentsForConstructor(Type typeOfFake, string reasonForFailure)
        {
            var message = new StringBuilder();

            message
                .AppendLine()
                .AppendIndented("  ", "Failed to create fake of type \"")
                .Append(typeOfFake)
                .AppendLine("\" with the specified arguments for the constructor:")
                .AppendIndented("    ", reasonForFailure)
                .AppendLine();

            throw new FakeCreationException(message.ToString());
        }

        public void ThrowFailedToGenerateProxyWithResolvedConstructors(Type typeOfFake, string reasonForFailureOfUnspecifiedConstructor, IEnumerable<ResolvedConstructor> resolvedConstructors)
        {
            var message = new StringBuilder();

            message
                .AppendLine()
                .AppendIndented("  ", "Failed to create fake of type \"")
                .Append(typeOfFake)
                .AppendLine("\".")
                .AppendLine()
                .AppendIndented("  ", "Below is a list of reasons for failure per attempted constructor:")
                .AppendLine()
                .AppendIndented("    ", "No constructor arguments failed:")
                .AppendLine()
                .AppendIndented("      ", reasonForFailureOfUnspecifiedConstructor)
                .AppendLine();

            if (resolvedConstructors.Any(x => x.WasSuccessfullyResolved))
            {
                foreach (var constructor in resolvedConstructors.Where(x => x.WasSuccessfullyResolved))
                {
                    message
                        .AppendIndented("    ", "Constructor with signature (")
                        .Append(constructor.Arguments.ToCollectionString(x => x.ArgumentType.ToString(), ", "))
                        .AppendLine(") failed:")
                        .AppendIndented("      ", constructor.ReasonForFailure)
                        .AppendLine();
                }
            }

            AppendNonTriedConstructors(resolvedConstructors, message);
            message
                .AppendLine();

            throw new FakeCreationException(message.ToString());
        }

        private static void AppendNonTriedConstructors(IEnumerable<ResolvedConstructor> resolvedConstructors, StringBuilder message)
        {
            if (resolvedConstructors.Any(x => !x.WasSuccessfullyResolved))
            {
                message
                    .AppendIndented("    ", "The following constructors were not tried:")
                    .AppendLine();

                foreach (var resolvedConstructor in resolvedConstructors.Where(x => !x.WasSuccessfullyResolved))
                {
                    message.Append("      (");
                    message.Append(resolvedConstructor.Arguments.ToCollectionString(x => x.WasResolved ? x.ArgumentType.ToString() : string.Concat("*", x.ArgumentType.ToString()), ", "));
                    message.AppendLine(")");
                }

                message
                    .AppendLine()
                    .AppendIndented("      ", "Types marked with * could not be resolved, register them in the current\r\nIFakeObjectContainer to enable these constructors.")
                    .AppendLine();
            }
        }
    }
}