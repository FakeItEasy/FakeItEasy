namespace FakeItEasy.Creation
{
    using System.Linq;

    internal class ResolvedConstructor
    {
        public bool WasSuccessfullyResolved => this.Arguments?.All(x => x.WasResolved) ?? true;

        public ResolvedArgument[] Arguments { get; set; }

        public string ReasonForFailure { get; set; }
    }
}
