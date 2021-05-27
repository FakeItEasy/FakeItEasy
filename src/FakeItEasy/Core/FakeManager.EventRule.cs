namespace FakeItEasy.Core
{
    public partial class FakeManager
    {
        private class EventRule
            : IFakeObjectCallRule
        {
            private readonly FakeManager fakeManager;

            private EventCallHandler? handler;

            public EventRule(FakeManager fakeManager)
            {
                this.fakeManager = fakeManager;
            }

            public int? NumberOfTimesToCall => null;

            private EventCallHandler Handler => this.handler ??= new EventCallHandler(this.fakeManager);

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                return EventCall.TryGetEventCall(fakeObjectCall, out _);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (EventCall.TryGetEventCall(fakeObjectCall, out var eventCall))
                {
                    this.Handler.HandleEventCall(eventCall);
                }
            }
        }
    }
}
