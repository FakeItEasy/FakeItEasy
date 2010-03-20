using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using FakeItEasy.Core;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class RaiseTests
    {
        IFoo foo;
        object sender;
        EventArgs eventArguments;

        [SetUp]
        public void SetUp()
        {
            this.foo = A.Fake<IFoo>();
            foo.SomethingHappened += new EventHandler(foo_SomethingHappened);
            this.sender = null;
            this.eventArguments = null;
        }

        void foo_SomethingHappened(object sender, EventArgs e)
        {
            this.sender = sender;
            this.eventArguments = e;
        }

        [Test]
        public void Raise_with_sender_and_arguments_should_raise_event_with_specified_sender()
        {
            object senderToUse = new object();
            this.foo.SomethingHappened += Raise.With(senderToUse, EventArgs.Empty).Now;

            Assert.That(this.sender, Is.EqualTo(senderToUse));
        }

        [Test]
        public void Raise_with_sender_and_arguments_should_raise_event_with_specified_arguments()
        {
            var arguments = new EventArgs();

            this.foo.SomethingHappened += Raise.With(this.foo, arguments).Now;

            Assert.That(this.eventArguments, Is.SameAs(arguments));
        }

        [Test]
        public void Raise_with_arguments_only_should_raise_event_with_fake_as_sender()
        {
            object senderToUse = new object();
            this.foo.SomethingHappened += Raise.With(EventArgs.Empty).Now;

            Assert.That(this.sender, Is.SameAs(this.foo));
        }

        [Test]
        public void Raise_with_arguments_only_should_raise_event_with_specified_arguments()
        {
            var arguments = new EventArgs();

            this.foo.SomethingHappened += Raise.With(arguments).Now;
             
            Assert.That(this.eventArguments, Is.SameAs(arguments));
        }

        [Test]
        public void Now_should_throw_when_called_directly()
        {
            var raiser = new Raise<EventArgs>(null, EventArgs.Empty);
            
            Assert.Throws<NotSupportedException>(() => 
                raiser.Now(null, null));
        }

        [Test]
        public void Go_should_return_handler_with_Now_as_method()
        {
            Assert.That(Raise.With(EventArgs.Empty).Go.Method.Name, Is.EqualTo("Now"));
        }

        [Test]
        public void WithEmpty_should_return_raise_object_with_event_args_emtpy_set()
        {
            var result = Raise.WithEmpty();

            var eventArgs = (result as IEventRaiserArguments).EventArguments;

            Assert.That(eventArgs, Is.EqualTo(EventArgs.Empty));
        }
    }
}
