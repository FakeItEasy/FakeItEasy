namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class CallWriter
    {
        public virtual void WriteCalls(int indent, IEnumerable<IFakeObjectCall> calls, TextWriter writer)
        {
            var indentString = CreateIndentString(indent);
            this.AppendCallList(indentString, calls, writer);
        }

        private static string CreateIndentString(int indent)
        {
            return string.Concat(Enumerable.Repeat(" ", indent).ToArray());
        }

        private void AppendCallList(string indentString, IEnumerable<IFakeObjectCall> calls, TextWriter writer)
        {
            var callDescriptions = new Queue<string>(calls.Select(x => x.ToString()));

            int callNumber = 0;
            int lineNumber = 1;
            while (callDescriptions.Count > 0 && lineNumber < 20)
            {
                var description = callDescriptions.Dequeue();
                callNumber++;
                WriteCallLine(indentString, writer, callNumber, description);

                int repeatOfCurrentCall = 1;

                while (callDescriptions.Count > 0 && string.Equals(description, callDescriptions.Peek(), StringComparison.Ordinal))
                {
                    repeatOfCurrentCall++;
                    callDescriptions.Dequeue();
                    callNumber++;
                }

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

                lineNumber++;
            }

            var nonPrintedCalls = callDescriptions.Count;
            if (nonPrintedCalls > 0)
            {
                writer.Write(indentString);
                writer.WriteLine("... Found {0} more calls not displayed here.", nonPrintedCalls);
            }
        }

        private static void WriteCallLine(string indentString, TextWriter writer, int callNumber, string callDescription)
        {
            writer.Write(indentString);

            WriteCallNumber(writer, callNumber);

            writer.Write("'");
            writer.Write(callDescription);
            writer.Write("'");
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
