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

        public virtual void WriteCalls(int indent, IEnumerable<IFakeObjectCall> calls, TextWriter writer)
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

            WriteCalls(indent, callInfos, writer);

            if (callArray.Length > MaxNumberOfCallsToWrite)
            {
                writer.WriteLine();
                writer.Write("... Found {0} more calls not displayed here.", callArray.Length - MaxNumberOfCallsToWrite);
            }

            writer.WriteLine();
        }

        private static void WriteCalls(int indent, IEnumerable<CallInfo> callInfos, TextWriter writer)
        {
            var lastCall = callInfos.Last();
            var numberOfDigitsInLastCallNumber = lastCall.NumberOfDigitsInCallNumber();
            var callDescriptionStartColumn = numberOfDigitsInLastCallNumber + 3 + indent;

            foreach (var call in callInfos)
            {
                if (call.CallNumber > 1)
                {
                    writer.WriteLine();
                }

                WriteIndentation(writer, indent);
                writer.Write(call.CallNumber);
                writer.Write(": ");

                WriteIndentation(writer, numberOfDigitsInLastCallNumber - call.NumberOfDigitsInCallNumber());

                WriteIndentedAtNewLine(writer, call.StringRepresentation, callDescriptionStartColumn);

                if (call.Repeat > 1)
                {
                    writer.Write(" repeated ");
                    writer.Write(call.Repeat);
                    writer.WriteLine(" times");
                    WriteIndentation(writer, indent);
                    writer.Write("...");
                }
            }
        }

        private static void WriteIndentedAtNewLine(TextWriter writer, string value, int indentLevel)
        {
            var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            writer.Write(lines[0]);

            for (var i = 1; i < lines.Length; i++)
            {
                writer.WriteLine();
                WriteIndentation(writer, indentLevel);
                writer.Write(lines[i]);
            }
        }

        private static void WriteIndentation(TextWriter writer, int indentLevel)
        {
            for (var i = 0; i < indentLevel; i++)
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