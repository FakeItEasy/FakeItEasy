namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Tests;
    
    internal class SyntaxTests
    {
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Required for testing.")]
        public void Test()
        {
            var foo = A.Fake<Foo>();

            A.CallTo(() => foo.Bar())
                .Returns("1");

            A.CallTo(() => foo.Bar())
                .Returns("foo");

            A.CallTo(() => foo.Baz())
                .Throws(new InvalidOperationException());

            A.CallTo(() => foo.Bar())
                .Returns("test")
                .Twice();

            A.CallTo(() => foo.Baz())
                .Throws(new ArgumentNullException("test"));

            A.CallTo(() => foo.Bar())
                .ReturnsLazily(x => "test");

            ////A.CallTo(() => foo).Baz()).RemoveCallConfigurations();
            ////A.CallTo(() => foo).RemoveFakeObjectConfigurations();

            // Expectations
            A.CallTo(() => foo.Bar())
                .Returns("test")
                .Once();

            A.CallTo(() => foo.Baz())
                .Throws(new ArgumentNullException())
                .Twice();

            A.CallTo(() => foo.Bar()).Throws(new ArgumentException()).NumberOfTimes(2);

            A.CallTo(() => foo.Bar(A<string>._, A<int>.That.Matches(p => p > 10)))
                .Returns("foo");

            foo = A.Fake<Foo>();
            ////A.CallTo(() => foo).Event(x => x.SomethingHappened += null)
            ////foo.SomethingHappened += new EventHandler(foo_SomethingHappened);
            ////Fake.VerifySetExpectations(foo);

            ////A.CallTo(() => foo).Event(x => x.SomethingHappened += null).Throws(new ArgumentNullException());
            ////A.CallTo(() => foo).Event(x => x.SomethingHappened += null).Raise(sender, EventArgs.Empty);

            ////var t = foo.SomethingHappened;
            ////foo.SomethingHappened += new EventManager<EventArgs>().Raise;
            ////var raiser = Raise.With(foo, EventArgs.Empty);
            ////foo.SomethingHappened += raiser.Now;
            ////var f = foo.SomethingHappened;
            ////foo.SomethingHappened += Raise.With(foo, EventArgs.Empty).Now;
            ////foo.SomethingHappened += Raise.With(EventArgs.Empty).Now;
        }
    }
}