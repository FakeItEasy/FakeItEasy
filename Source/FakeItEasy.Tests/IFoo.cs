namespace FakeItEasy.Tests
{
    using System;

    public interface IFoo
    {
        event EventHandler SomethingHappened;

        int SomeProperty { get; set; }

        string ReadOnlyProperty { get; }

        string WriteOnlyProperty { set; }

        IFoo ChildFoo { get; }

        int this[int index] { get; set; }

        void Bar();

        void Bar(object argument);

        void Bar(object argument, object argument2);

        void Bar(object argument1, object argument2, object argument3);

        int Baz();

        int Baz(object argument, object argument2);

        object Biz();
    }
}