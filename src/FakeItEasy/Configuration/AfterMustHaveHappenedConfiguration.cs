namespace FakeItEasy.Configuration
{
    using FakeItEasy.Core;

    internal class AfterMustHaveHappenedConfiguration : IAfterMustHaveHappenedConfiguration
    {
        private readonly FakeManager fakeManager;
        private readonly ICallMatcher matcher;
        private readonly string callDescription;
        private readonly Repeated repeatConstraint;

        public AfterMustHaveHappenedConfiguration(FakeManager fakeManager, ICallMatcher matcher, string callDescription, Repeated repeatConstraint)
        {
            this.fakeManager = fakeManager;
            this.matcher = matcher;
            this.callDescription = callDescription;
            this.repeatConstraint = repeatConstraint;
        }

        public void InOrder(ISequentialCallContext context)
        {
            Guard.AgainstNull(context, "context");

            context.CheckNextCall(this.fakeManager, this.matcher.Matches, this.callDescription, this.repeatConstraint);
        }
    }
}
