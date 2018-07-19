namespace FakeItEasy.Creation
{
    using System.Linq;

    internal class ResolvedConstructor
    {
        public ResolvedConstructor(ResolvedArgument[] resolvedArguments)
        {
            Guard.AgainstNull(resolvedArguments, nameof(resolvedArguments));
            this.Arguments = resolvedArguments;
        }

        public bool WasSuccessfullyResolved => this.Arguments.All(x => x.WasResolved);

        public ResolvedArgument[] Arguments { get; }

        public string ReasonForFailure { get; set; }
    }
}
