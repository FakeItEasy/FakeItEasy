namespace FakeItEasy.Core
{
    using System;
    using static FakeItEasy.ObjectMethod;

    internal class StrictFakeRule : IFakeObjectCallRule
    {
        private readonly StrictFakeOptions options;

        public StrictFakeRule(StrictFakeOptions options)
        {
            this.options = options;
        }

        public int? NumberOfTimesToCall => null;

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            var objectMethod = fakeObjectCall.Method.GetObjectMethod();

            if (objectMethod == EqualsMethod)
            {
                return !this.HasOption(StrictFakeOptions.AllowEquals);
            }

            if (objectMethod == GetHashCodeMethod)
            {
                return !this.HasOption(StrictFakeOptions.AllowGetHashCode);
            }

            if (objectMethod == ToStringMethod)
            {
                return !this.HasOption(StrictFakeOptions.AllowToString);
            }

            if (EventCall.TryGetEventCall(fakeObjectCall, out _))
            {
                return !this.HasOption(StrictFakeOptions.AllowEvents);
            }

            return true;
        }

        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            string message = ExceptionMessages.CallToUnconfiguredMethodOfStrictFake(fakeObjectCall);

            if (EventCall.TryGetEventCall(fakeObjectCall, out _))
            {
                message += Environment.NewLine + ExceptionMessages.HandleEventsOnStrictFakes;
            }

            throw new ExpectationException(message);
        }

        private bool HasOption(StrictFakeOptions flag) => (flag & this.options) == flag;
    }
}
