namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xbehave;

    public static class ObjectCallSpecs
    {
        public interface IFoo
        {
            void VoidMethod();

            int IntMethod();
        }

        [Scenario]
        public static void WriteOneCall(IFoo fake, IEnumerable<IFakeObjectCall> calls, StringOutputWriter writer)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And an output writer"
                .x(() => writer = new StringOutputWriter());

            "And I make a call to the fake"
                .x(() => fake.VoidMethod());

            "And I query the fake for its calls"
                .x(() => calls = Fake.GetCalls(fake));

#pragma warning disable CS0618 // Type or member is obsolete
            "When I write the calls to the output writer"
                .x(() => calls.Write(writer));
#pragma warning restore CS0618 // Type or member is obsolete

            "Then the output lists the call"
                .x(() => writer.ToString().Should().Be("1: FakeItEasy.Specs.ObjectCallSpecs+IFoo.VoidMethod()\r\n"));
        }

        [Scenario]
        public static void WriteTwoCalls(IFoo fake, IEnumerable<IFakeObjectCall> calls, StringOutputWriter writer)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And an output writer"
                .x(() => writer = new StringOutputWriter());

            "And I make a call to the fake"
                .x(() => fake.VoidMethod());

            "And I make a different call to the fake"
                .x(() => fake.IntMethod());

            "And I query the fake for its calls"
                .x(() => calls = Fake.GetCalls(fake));

#pragma warning disable CS0618 // Type or member is obsolete
            "When I write the calls to the output writer"
                .x(() => calls.Write(writer));
#pragma warning restore CS0618 // Type or member is obsolete

            "Then the output lists the calls"
                .x(() => writer.ToString().Should().Be(
                    "1: FakeItEasy.Specs.ObjectCallSpecs+IFoo.VoidMethod()\r\n" +
                    "2: FakeItEasy.Specs.ObjectCallSpecs+IFoo.IntMethod()\r\n"));
        }

        public class StringOutputWriter : IOutputWriter
        {
            private readonly StringBuilder builder;

            public StringOutputWriter()
            {
                this.builder = new StringBuilder();
            }

            public IOutputWriter Write(string value)
            {
                this.builder.Append(value);
                return this;
            }

            public IOutputWriter WriteArgumentValue(object value)
            {
                return this.Write(value.ToString());
            }

            public IDisposable Indent()
            {
                // no tests require indenting yet
                return A.Dummy<IDisposable>();
            }

            public override string ToString()
            {
                return this.builder.ToString();
            }
        }
    }
}
