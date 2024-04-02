namespace FakeItEasy.Core;

using System.Linq;

public partial class FakeManager
{
    private class FakeSnapshot
    {
        private readonly CallRuleMetadata[]? userRules;
        private readonly EventCallHandler? eventCallHandler;
        private readonly CompletedFakeObjectCall[]? recordedCalls;
        private readonly IInterceptionListener[]? interceptionListeners;

        private FakeSnapshot(
            CallRuleMetadata[]? userRules,
            EventCallHandler? eventCallHandler,
            CompletedFakeObjectCall[]? recordedCalls,
            IInterceptionListener[]? interceptionListeners)
        {
            this.userRules = userRules;
            this.eventCallHandler = eventCallHandler;
            this.recordedCalls = recordedCalls;
            this.interceptionListeners = interceptionListeners;
        }

        public static FakeSnapshot CaptureState(FakeManager fakeManager)
        {
            var userRules = CaptureUserRules(fakeManager);
            var eventCallHandler = fakeManager.eventCallHandler?.GetSnapshot();
            var recordedCalls = CaptureRecordedCalls(fakeManager);
            var interceptionListeners = CaptureInterceptionListeners(fakeManager);
            return new FakeSnapshot(userRules, eventCallHandler, recordedCalls, interceptionListeners);
        }

        public void RestoreState(FakeManager fakeManager)
        {
            fakeManager.ReplaceUserRules(this.userRules?.Select(r => r.GetSnapshot()));
            fakeManager.eventCallHandler = this.eventCallHandler?.GetSnapshot();
            fakeManager.ReplaceRecordedCalls(this.recordedCalls);
            fakeManager.ReplaceInterceptionListeners(this.interceptionListeners);
        }

        private static CallRuleMetadata[]? CaptureUserRules(FakeManager fakeManager)
        {
            if (fakeManager.allUserRules.Count == 0)
            {
                return null;
            }

            return fakeManager.allUserRules
                .Select(r => r.GetSnapshot())
                .ToArray();
        }

        private static CompletedFakeObjectCall[]? CaptureRecordedCalls(FakeManager fakeManager)
        {
            if (fakeManager.recordedCalls.IsEmpty)
            {
                return null;
            }

            return fakeManager.recordedCalls.ToArray();
        }

        private static IInterceptionListener[]? CaptureInterceptionListeners(FakeManager fakeManager)
        {
            if (fakeManager.interceptionListeners.Count == 0)
            {
                return null;
            }

            return fakeManager.interceptionListeners.ToArray();
        }
    }
}
