namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Core;

    public sealed class FakeCallRule : IFakeObjectCallRule
    {
        public Func<IFakeObjectCall, bool> IsApplicableTo { get; set; }

        public Action<IInterceptedFakeObjectCall> Apply { get; set; }

        public bool ApplyWasCalled { get; set; }

        public bool IsApplicableToWasCalled { get; set; }

        public int? NumberOfTimesToCall { get; set; }

        public bool MayNotBeCalledMoreThanTheNumberOfTimesSpecified { get; set; }

        public bool MustGetCalled { get; set; }

        bool IFakeObjectCallRule.IsApplicableTo(IFakeObjectCall invocation)
        {
            this.IsApplicableToWasCalled = true;
            return this.IsApplicableTo != null ? this.IsApplicableTo(invocation) : false;
        }

        void IFakeObjectCallRule.Apply(IInterceptedFakeObjectCall invocation)
        {
            this.ApplyWasCalled = true;

            if (this.Apply != null)
            {
                this.Apply(invocation);
            }
        }
    }
}
