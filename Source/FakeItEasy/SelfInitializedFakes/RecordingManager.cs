namespace FakeItEasy.SelfInitializedFakes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy.Core;

    /// <summary>
    /// Manages the applying of recorded calls and recording of new calls when
    /// using self initialized fakes.
    /// </summary>
    public class RecordingManager
        : ISelfInitializingFakeRecorder
    {
        #region Nested types
        /// <summary>
        /// Represents a factory responsible for creating recording manager
        /// instances.
        /// </summary>
        /// <param name="storage">The storage the manager should use.</param>
        /// <returns>A RecordingManager instance.</returns>
        internal delegate RecordingManager Factory(ICallStorage storage);

        private class CallDataMetadata
        {
            public CallData RecordedCall;
            public bool HasBeenApplied;
            public override string ToString()
            {
                return new StringBuilder()
                    .AppendFormat("Applied: {0}", this.HasBeenApplied)
                    .AppendLine()
                    .Append(this.RecordedCall.Method.Name)
                    .Append(" ")
                    .Append(RecordedCall.ReturnValue)
                    .ToString();
            }
        } 
        #endregion

        #region Fields
        private ICallStorage storage;
        private Queue<CallDataMetadata> callQueue;
        private List<CallDataMetadata> recordedCalls;
        #endregion

        #region Construction
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        public RecordingManager(ICallStorage storage)
        {
            this.storage = storage;

            var recordedCalls = storage.Load();
            
            this.IsRecording = recordedCalls == null;
            this.callQueue = CreateCallsList(recordedCalls);
            this.recordedCalls = new List<CallDataMetadata>(this.callQueue);
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating if the recorder is currently recording.
        /// </summary>
        /// <value></value>
        public bool IsRecording
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Applies the call if the call has been recorded.
        /// </summary>
        /// <param name="call">The call to apply to from recording.</param>
        public void ApplyNext(IWritableFakeObjectCall call)
        {
            this.AssertThatCallQueueIsNotEmpty();

            var callToApply = this.callQueue.Dequeue();

            AssertThatMethodsMatches(call, callToApply);
            ApplyOutputArguments(call, callToApply);

            call.SetReturnValue(callToApply.RecordedCall.ReturnValue);
            callToApply.HasBeenApplied = true;
        }

        private static void ApplyOutputArguments(IWritableFakeObjectCall call, CallDataMetadata callToApply)
        {
            foreach (var outputArgument in GetIndicesAndValuesOfOutputParameters(call, callToApply.RecordedCall))
            {
                call.SetArgumentValue(outputArgument.First, outputArgument.Second);
            }
        }

        private void AssertThatCallQueueIsNotEmpty()
        {
            if (this.callQueue.Count == 0)
            {
                throw new RecordingException(ExceptionMessages.NoMoreRecordedCalls);
            }
        }

        private static void AssertThatMethodsMatches(IWritableFakeObjectCall call, CallDataMetadata callToApply)
        {
            if (!callToApply.RecordedCall.Method.Equals(call.Method))
            {
                throw new RecordingException(ExceptionMessages.MethodMissmatchWhenPlayingBackRecording);
            }
        }

        /// <summary>
        /// Records the specified call.
        /// </summary>
        /// <param name="call">The call to record.</param>
        public virtual void RecordCall(ICompletedFakeObjectCall call)
        {
            var callData = new CallData(call.Method, GetOutputArgumentsForCall(call), call.ReturnValue);
            this.recordedCalls.Add(new CallDataMetadata { HasBeenApplied = true, RecordedCall = callData });
        }

        /// <summary>
        /// Saves all recorded calls to the storage.
        /// </summary>
        public virtual void Dispose()
        {
            this.storage.Save(this.recordedCalls.Select(x => x.RecordedCall));
        }

        private static IEnumerable<object> GetOutputArgumentsForCall(IFakeObjectCall call)
        {
            return
                from valueAndParameterInfo in call.Method.GetParameters().Zip(call.Arguments.AsEnumerable())
                where valueAndParameterInfo.First.ParameterType.IsByRef
                select valueAndParameterInfo.Second;
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

        private static IEnumerable<Tuple<int, object>> GetIndicesAndValuesOfOutputParameters(IWritableFakeObjectCall call, CallData recordedCall)
        {
            return
                (from argument in call.Method.GetParameters().Zip(Enumerable.Range(0, int.MaxValue))
                 where argument.First.ParameterType.IsByRef
                 select argument.Second).Zip(recordedCall.OutputArguments);
        } 
        #endregion
    }
}