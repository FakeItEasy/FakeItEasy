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

            AppendInternalHint(typeOfFake, message);

            message
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
            AppendInternalHint(typeOfFake, message);

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

        private static void AppendInternalHint(Type typeOfFake, StringBuilder message)
        {
            // NOTE (adamralph): I wanted to use typeOfFake.Assembly.GetName().GetPublicKey().Length > 0 but Assembly.GetName() is marked SecurityCritical
            var publicKeyText = typeOfFake.Assembly.FullName.Contains("PublicKeyToken=null")
                ? string.Empty
                : ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7";

            message
                .AppendLine()
                .AppendIndented("  ", "If either the type or constructor is internal, try adding the following attribute to the assembly:")
                .AppendLine()
                .AppendIndented("    ", string.Concat("[assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2", publicKeyText, "\")]"))
                .AppendLine();
        }
    }
}