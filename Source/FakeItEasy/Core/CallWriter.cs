namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class CallWriter
    {
        private const int MaxNumberOfCallsToWrite = 19;
        private readonly IFakeObjectCallFormatter callFormatter;

        public CallWriter(IFakeObjectCallFormatter callFormatter)
        {
            this.callFormatter = callFormatter;
        }

        public virtual void WriteCalls(int indent, IEnumerable<IFakeObjectCall> calls, TextWriter writer)
        {
            if (!calls.Any())
            {
                return;
            }

            var callInfos = new List<CallInfo>(new[] {new CallInfo { CallNumber = 0, Repeat = 1, StringRepresentation = "NullCall" }});
            var callArray = calls.ToArray();

            for (int i = 0; i < callArray.Length && i < MaxNumberOfCallsToWrite; i++)
            {
                var call = callArray[i];

                var lastCall = callInfos[callInfos.Count - 1];
                var description = this.callFormatter.GetDescription(call);

                if (description == lastCall.StringRepresentation)
                {
                    lastCall.Repeat++;
                }
                else
                {
                    callInfos.Add(new CallInfo()
                                      {
                                          CallNumber = lastCall.CallNumber + lastCall.Repeat,
                                          StringRepresentation = description
                                      });
                }
            }

            callInfos.RemoveAt(0);

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

            for (int i = 1; i < lines.Length; i++)
            {
                writer.WriteLine();
                WriteIndentation(writer, indentLevel);
                writer.Write(lines[i]);
            }
        }

        private static void WriteIndentation(TextWriter writer, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++)
            {
                writer.Write(" ");
            } 
        }

        private class CallInfo
        {
            public int CallNumber;
            public int Repeat = 1;
            public string StringRepresentation;

            public int NumberOfDigitsInCallNumber()
            {
                return (int)Math.Log10(this.CallNumber);
            }
        }
    }
}