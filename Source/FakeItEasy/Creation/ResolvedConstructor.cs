namespace FakeItEasy.Creation
{
    using System.Linq;

    internal class ResolvedConstructor
    {
        public bool WasSuccessfullyResolved
        {
            get { return !this.Arguments.Any(x => !x.WasResolved); }
        }

        public ResolvedArgument[] Arguments { get; set; }

        public string ReasonForFailure { get; set; }
    }
}
