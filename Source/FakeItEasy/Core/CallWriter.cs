namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class CallWriter
    {
        private IFakeObjectCallFormatter callFormatter;

        public CallWriter(IFakeObjectCallFormatter callFormatter)
        {
            this.callFormatter = callFormatter;
        }

        public virtual void WriteCalls(int indent, IEnumerable<IFakeObjectCall> calls, TextWriter writer)
        {
            var indentString = CreateIndentString(indent);
            AppendCallList(indentString, calls, writer);
        }

        private static string CreateIndentString(int indent)
        {
            return string.Concat(Enumerable.Repeat(" ", indent).ToArray());
        }

        private void AppendCallList(string indentString, IEnumerable<IFakeObjectCall> calls, TextWriter writer)
        {
            var callDescriptions = new Queue<string>(calls.Select(x => this.callFormatter.GetDescription(x)));

            int callNumber = 0;
            int lineNumber = 1;

            while (callDescriptions.Count > 0 && lineNumber < 20)
            {
                callNumber++;
                
                var description = callDescriptions.Dequeue();
                var repeatOfCurrentCall = GetRepeatOfCurrentCallAndMoveToNextCall(callDescriptions, description);

                WriteCallDescription(indentString, writer, callNumber, description);
                AppendRepeatAndEndCallDescription(indentString, writer, repeatOfCurrentCall);
                
                callNumber += repeatOfCurrentCall - 1;
                lineNumber++;
            }

            var nonPrintedCalls = callDescriptions.Count;
            if (nonPrintedCalls > 0)
            {
                writer.Write(indentString);
                writer.WriteLine("... Found {0} more calls not displayed here.", nonPrintedCalls);
            }
        }

        private static void AppendRepeatAndEndCallDescription(string indentString, TextWriter writer, int repeatOfCurrentCall)
        {
            if (repeatOfCurrentCall > 1)
            {
                writer.WriteLine(" repeated {0} times", repeatOfCurrentCall);
                writer.Write(indentString);
                writer.WriteLine("...");
            }
            else
            {
                writer.WriteLine();
            }
        }

        private static int GetRepeatOfCurrentCallAndMoveToNextCall(Queue<string> callDescriptions, string description)
        {
            int repeatOfCurrentCall = 1;

            while (callDescriptions.Count > 0 && string.Equals(description, callDescriptions.Peek(), StringComparison.Ordinal))
            {
                repeatOfCurrentCall++;
                callDescriptions.Dequeue();
            }

            return repeatOfCurrentCall;
        }

        private static void WriteCallDescription(string indentString, TextWriter writer, int callNumber, string callDescription)
        {
            writer.Write(indentString);

            WriteCallNumber(writer, callNumber);

            callDescription = IndentNewlinesInCallDescription(indentString, callDescription);

            writer.Write("'");
            writer.Write(callDescription);
            writer.Write("'");
        }

        private static string IndentNewlinesInCallDescription(string indentString, string callDescription)
        {
            return callDescription.Replace("\r\n", "\r\n" + indentString + "    ");
        }

        private static void WriteCallNumber(TextWriter writer, int callNumber)
        {
            writer.Write(callNumber);
            writer.Write(". ");

            if (callNumber < 10)
            {
                writer.Write(" ");
            }
        }
    }
}