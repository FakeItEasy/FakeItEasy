namespace FakeItEasy.SelfInitializedFakes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using FakeItEasy.Core;

    /// <summary>
    /// Manages the applying of recorded calls and recording of new calls when
    /// using self initialized fakes.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Implements the Disposable method to support the using statement only.")]
    [Obsolete("Self-initializing fakes will be removed in version 4.0.0.")]
    public class RecordingManager : ISelfInitializingFakeRecorder
    {
        private readonly Queue<CallDataMetadata> callQueue;
        private readonly List<CallDataMetadata> recordedCalls;
        private readonly ICallStorage storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        public RecordingManager(ICallStorage storage)
        {
            Guard.AgainstNull(storage, nameof(storage));

            this.storage = storage;

            var callsFromStorage = storage.Load();

            this.IsRecording = callsFromStorage == null;
            this.callQueue = CreateCallsList(callsFromStorage);
            this.recordedCalls = new List<CallDataMetadata>(this.callQueue);
        }

        /// <summary>
        /// Represents a factory responsible for creating recording manager
        /// instances.
        /// </summary>
        /// <param name="storage">The storage the manager should use.</param>
        /// <returns>A RecordingManager instance.</returns>
        internal delegate RecordingManager Factory(ICallStorage storage);

        /// <summary>
        /// Gets a value indicating whether the recorder is currently recording.
        /// </summary>
        /// <value></value>
        public bool IsRecording { get; }

        /// <summary>
        /// Applies the call if the call has been recorded.
        /// </summary>
        /// <param name="fakeObjectCall">The call to apply to from recording.</param>
        public void ApplyNext(IInterceptedFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

            this.AssertThatCallQueueIsNotEmpty();

            var callToApply = this.callQueue.Dequeue();

            AssertThatMethodsMatches(fakeObjectCall, callToApply);
            ApplyOutputArguments(fakeObjectCall, callToApply);

            fakeObjectCall.SetReturnValue(callToApply.RecordedCall.ReturnValue);
            callToApply.HasBeenApplied = true;
        }

        /// <summary>
        /// Records the specified call.
        /// </summary>
        /// <param name="fakeObjectCall">The call to record.</param>
        public virtual void RecordCall(ICompletedFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

            var callData = new CallData(fakeObjectCall.Method, GetOutputArgumentsForCall(fakeObjectCall), fakeObjectCall.ReturnValue);
            this.recordedCalls.Add(new CallDataMetadata { HasBeenApplied = true, RecordedCall = callData });
        }

        /// <summary>
        /// Saves all recorded calls to the storage.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "Does not have a finalizer.")]
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "The dispose method is provided for enabling using statement only, virtual for testability.")]
        public virtual void Dispose()
        {
            this.storage.Save(this.recordedCalls.Select(x => x.RecordedCall));
        }

        private static void ApplyOutputArguments(IInterceptedFakeObjectCall call, CallDataMetadata callToApply)
        {
            foreach (var outputArgument in call.Method.GetParameters()
                .Select((parameter, index) => new { Parameter = parameter, Index = index })
                .Where(argument => argument.Parameter.ParameterType.IsByRef)
                .Select(argument => argument.Index).Zip(callToApply.RecordedCall.OutputArguments, (index, outputArgument) => new { Index = index, Value = outputArgument }))
            {
                call.SetArgumentValue(outputArgument.Index, outputArgument.Value);
            }
        }

        private static void AssertThatMethodsMatches(IInterceptedFakeObjectCall call, CallDataMetadata callToApply)
        {
            if (!callToApply.RecordedCall.Method.Equals(call.Method))
            {
                throw new RecordingException(ExceptionMessages.MethodMissmatchWhenPlayingBackRecording);
            }
        }

        private static IEnumerable<object> GetOutputArgumentsForCall(IFakeObjectCall call)
        {
            return
                from valueAndParameterInfo in call.Method.GetParameters().Zip(
                    call.Arguments.AsEnumerable(),
                    (parameter, argument) => new { parameter.ParameterType, Argument = argument })
                where valueAndParameterInfo.ParameterType.IsByRef
                select valueAndParameterInfo.Argument;
        }

        private static Queue<CallDataMetadata> CreateCallsList(IEnumerable<CallData> callsFromStorage)
        {
            if (callsFromStorage == null)
            {
                return new Queue<CallDataMetadata>();
            }

            var result = new Queue<CallDataMetadata>();
            foreach (var call in callsFromStorage)
            {
                result.Enqueue(new CallDataMetadata { RecordedCall = call });
            }

            return result;
        }

        private void AssertThatCallQueueIsNotEmpty()
        {
            if (this.callQueue.Count == 0)
            {
                throw new RecordingException(ExceptionMessages.NoMoreRecordedCalls);
            }
        }

        private class CallDataMetadata
        {
            public CallData RecordedCall { get; set; }

            public bool HasBeenApplied { get; set; }

            public override string ToString()
            {
                return new StringBuilder()
                    .Append("Applied: ")
                    .Append(this.HasBeenApplied)
                    .AppendLine()
                    .Append(this.RecordedCall.Method.Name)
                    .Append(" ")
                    .Append(this.RecordedCall.ReturnValue)
                    .ToString();
            }
        }
    }
}
