using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;

namespace FakeItEasy
{
    class SyntaxTests
    {
        public void Test()
        {
            var foo = A.Fake<Foo>();

            
            
            Configure.Fake(foo).AnyCall().Throws(new ArgumentNullException());

            Configure.Fake(foo)
                .CallsTo(x => x.Bar())
                .Returns("1");

            Configure.Fake(foo)
                .CallsTo(x => x.Bar())
                .Returns("foo");

            Configure.Fake(foo)
                .CallsTo(x => x.Baz())
                .Throws(new Exception());

            Configure.Fake(foo)
                .CallsTo(x => x.Bar())
                .Returns("test")
                .Twice();

            
            Configure.Fake(foo)
                .CallsTo(x => x.Baz())
                .Throws(new ArgumentNullException("test"));

            
            Configure.Fake(foo)
                .CallsTo(x => x.Bar())
                .Returns(() => "test");

            //Configure.Fake(foo).CallsTo(x => x.Baz()).RemoveCallConfigurations();
            //Configure.Fake(foo).RemoveFakeObjectConfigurations();

            //// Expectations
            Configure.Fake(foo)
                .CallsTo(x => x.Bar())
                .Returns("test")
                .Once();

            Configure.Fake(foo)
                .CallsTo(x => x.Baz())
                .Throws(new ArgumentNullException())
                .Twice();

            

            Configure.Fake(foo).CallsTo(x => x.Bar()).Throws(new ArgumentException()).NumberOfTimes(2);

            Configure.Fake(foo)
                .CallsTo(x => x.Bar(A<string>.Ignored, A<int>.That.Matches(p => p > 10)))
                .Returns("foo");
            

            foo = A.Fake<Foo>();
            //Configure.Fake(foo).Event(x => x.SomethingHappened += null)
            //foo.SomethingHappened += new EventHandler(foo_SomethingHappened);
            //Fake.VerifySetExpectations(foo);
            
            //Configure.Fake(foo).Event(x => x.SomethingHappened += null).Throws(new ArgumentNullException());
            //Configure.Fake(foo).Event(x => x.SomethingHappened += null).Raise(sender, EventArgs.Empty);

            //var t = foo.SomethingHappened;
            //foo.SomethingHappened += new EventManager<EventArgs>().Raise;
            //var raiser = Raise.With(foo, EventArgs.Empty);
            //foo.SomethingHappened += raiser.Now;
            //var f = foo.SomethingHappened;
            //foo.SomethingHappened += Raise.With(foo, EventArgs.Empty).Now;
            //foo.SomethingHappened += Raise.With(EventArgs.Empty).Now;
        }
    }
}