namespace FakeItEasy.Core;

public partial class FakeManager
{
    private class EventRule
        : SharedFakeObjectCallRule
    {
        public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall);

            return EventCall.TryGetEventCall(fakeObjectCall, out _);
        }

        public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            if (EventCall.TryGetEventCall(fakeObjectCall, out var eventCall))
            {
                var fakeManager = Fake.GetFakeManager(fakeObjectCall.FakedObject);
                fakeManager.EventCallHandler.HandleEventCall(eventCall);
            }
        }
    }
}
