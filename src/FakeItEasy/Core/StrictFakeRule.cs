namespace FakeItEasy.Core
{
    internal class StrictFakeRule : IFakeObjectCallRule
    {
        public int? NumberOfTimesToCall => null;

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall) => true;

        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            throw new ExpectationException(ExceptionMessages.CallToUnconfiguredMethodOfStrictFake(fakeObjectCall));
        }
    }
}
