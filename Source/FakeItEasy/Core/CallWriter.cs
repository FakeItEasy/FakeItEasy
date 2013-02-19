namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class CallWriter
    {
        private const int MaxNumberOfCallsToWrite = 19;
        private readonly IEqualityComparer<IFakeObjectCall> callComparer;
        private readonly IFakeObjectCallFormatter callFormatter;

        public CallWriter(IFakeObjectCallFormatter callFormatter, IEqualityComparer<IFakeObjectCall> callComparer)
        {
            this.callFormatter = callFormatter;
            this.callComparer = callComparer;
        }

        public virtual void WriteCalls(IEnumerable<IFakeObjectCall> calls, IOutputWriter writer)
        {
            if (!calls.Any())
            {
                return;
            }

            var callInfos = new List<CallInfo>();
            var callArray = calls.ToArray();

            for (var i = 0; i < callArray.Length && i < MaxNumberOfCallsToWrite; i++)
            {
                var call = callArray[i];

                if (i > 0 && this.callComparer.Equals(callInfos[callInfos.Count - 1].Call, call))
                {
                    callInfos[callInfos.Count - 1].Repeat++;
                }
                else
                {
                    callInfos.Add(new CallInfo
                                      {
                                          Call = call, 
                                          CallNumber = i + 1, 
                                          StringRepresentation = this.callFormatter.GetDescription(call)
                                      });
                }
            }

            WriteCalls(callInfos, writer);

            if (callArray.Length > MaxNumberOfCallsToWrite)
            {
                writer.WriteLine();
                writer.Write("... Found {0} more calls not displayed here.", callArray.Length - MaxNumberOfCallsToWrite);
            }

            writer.WriteLine();
        }

        private static void WriteCalls(IEnumerable<CallInfo> callInfos, IOutputWriter writer)
        {
            var lastCall = callInfos.Last();
            var numberOfDigitsInLastCallNumber = lastCall.NumberOfDigitsInCallNumber();
            
            foreach (var call in callInfos)
            {
                if (call.CallNumber > 1)
                {
                    writer.WriteLine();
                }

                writer.Write(call.CallNumber);
                writer.Write(": ");

                WriteSpaces(writer, numberOfDigitsInLastCallNumber - call.NumberOfDigitsInCallNumber());

                using (writer.Indent())
                {
                    writer.Write(call.StringRepresentation);
                }
                
                if (call.Repeat > 1)
                {
                    writer.Write(" repeated ");
                    writer.Write(call.Repeat);
                    writer.Write(" times");
                    writer.WriteLine();
                    writer.Write("...");
                }
            }
        }

        private static void WriteSpaces(IOutputWriter writer, int numberOfSpaces)
        {
            for (var i = 0; i < numberOfSpaces; i++)
            {
                writer.Write(" ");
            }
        }

        private class CallInfo
        {
            public CallInfo()
            {
                this.Repeat = 1;
            }

            public IFakeObjectCall Call { get; set; }

            public int CallNumber { get; set; }

            public int Repeat { get; set; }

            public string StringRepresentation { get; set; }

            public int NumberOfDigitsInCallNumber()
            {
                return (int)Math.Log10(this.CallNumber);
            }
        }
    }
}