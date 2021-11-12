namespace FakeItEasy.Core
{
    public partial class FakeManager
    {
        private class EventRule
            : IFakeObjectCallRule
        {
            private readonly FakeManager fakeManager;

            public EventRule(FakeManager fakeManager)
            {
                this.fakeManager = fakeManager;
            }

            public int? NumberOfTimesToCall => null;

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall);

                return EventCall.TryGetEventCall(fakeObjectCall, out _);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (EventCall.TryGetEventCall(fakeObjectCall, out var eventCall))
                {
                    this.fakeManager.EventCallHandler.HandleEventCall(eventCall);
                }
            }
        }
    }
}
