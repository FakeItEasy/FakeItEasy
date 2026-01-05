namespace FakeItEasy.Core;

using System;
using System.Collections.Generic;
using System.Linq;

internal class SequentialCallContext
{
    private readonly CallWriter callWriter;
    private readonly StringBuilderOutputWriter.Factory outputWriterFactory;
    private readonly HashSet<FakeManager> fakeManagers;
    private readonly List<AssertedCall> assertedCalls;
    private int currentSequenceNumber;

    public SequentialCallContext(CallWriter callWriter, StringBuilderOutputWriter.Factory outputWriterFactory)
    {
        Guard.AgainstNull(callWriter);
        this.callWriter = callWriter;
        this.outputWriterFactory = outputWriterFactory;
        this.fakeManagers = new HashSet<FakeManager>();
        this.assertedCalls = new List<AssertedCall>();
        this.currentSequenceNumber = -1;
    }

    public delegate SequentialCallContext Factory();

    public void CheckNextCall(
        FakeManager fakeManager,
        Func<IFakeObjectCall, bool> callPredicate,
        Action<IOutputWriter> callDescriber,
        CallCountConstraint callCountConstraint)
    {
        Guard.AgainstNull(fakeManager);
        Guard.AgainstNull(callPredicate);
        Guard.AgainstNull(callDescriber);
        Guard.AgainstNull(callCountConstraint);
        this.fakeManagers.Add(fakeManager);
        this.assertedCalls.Add(
            new AssertedCall { CallDescriber = callDescriber, MatchingCountDescription = callCountConstraint.ToString() });

        var allCalls = this.fakeManagers.SelectMany(f => f.GetRecordedCalls()).OrderBy(call => call.SequenceNumber).ToList();

        int matchedCallCount = 0;
        foreach (var currentCall in allCalls.SkipWhile(c => c.SequenceNumber <= this.currentSequenceNumber))
        {
            if (callCountConstraint.Matches(matchedCallCount))
            {
                return;
            }

            if (callPredicate(currentCall))
            {
                matchedCallCount++;
                this.currentSequenceNumber = currentCall.SequenceNumber;
            }
        }

        if (!callCountConstraint.Matches(matchedCallCount))
        {
            this.ThrowExceptionWhenAssertionFailed(allCalls);
        }
    }

    private void ThrowExceptionWhenAssertionFailed(List<CompletedFakeObjectCall> originalCallList)
    {
        var message = this.outputWriterFactory.Invoke();

        message.WriteLine();
        message.WriteLine();

        using (message.Indent())
        {
            message.Write("Assertion failed for the following calls:");
            message.WriteLine();

            using (message.Indent())
            {
                foreach (var call in this.assertedCalls)
                {
                    message.Write("'");
                    call.CallDescriber.Invoke(message);
                    message.Write("' ");
                    message.Write(call.MatchingCountDescription);
                    message.WriteLine();
                }
            }

            message.Write("The calls were found but not in the correct order among the calls:");
            message.WriteLine();

            using (message.Indent())
            {
                this.callWriter.WriteCalls(originalCallList, message);
            }
        }

        throw new ExpectationException(message.Builder.ToString());
    }

    private struct AssertedCall
    {
        public Action<IOutputWriter> CallDescriber;
        public string MatchingCountDescription;
    }
}
