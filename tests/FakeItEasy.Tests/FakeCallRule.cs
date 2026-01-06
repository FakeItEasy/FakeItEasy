namespace FakeItEasy.Tests;

using System;
using FakeItEasy.Core;

public sealed class FakeCallRule : IFakeObjectCallRule
{
    public Func<IFakeObjectCall, bool> IsApplicableTo { get; set; } = call => true;

    public Action<IInterceptedFakeObjectCall> Apply { get; set; } = call => { };

    public bool ApplyWasCalled { get; set; }

    public bool IsApplicableToWasCalled { get; set; }

    public int? NumberOfTimesToCall { get; set; }

    public bool MayNotBeCalledMoreThanTheNumberOfTimesSpecified { get; set; }

    public bool MustGetCalled { get; set; }

    bool IFakeObjectCallRule.IsApplicableTo(IFakeObjectCall invocation)
    {
        this.IsApplicableToWasCalled = true;
        return this.IsApplicableTo(invocation);
    }

    void IFakeObjectCallRule.Apply(IInterceptedFakeObjectCall invocation)
    {
        this.ApplyWasCalled = true;
        this.Apply(invocation);
    }
}
