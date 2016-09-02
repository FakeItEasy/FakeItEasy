namespace FakeItEasy.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IFoo
    {
        event EventHandler SomethingHappened;

        int SomeProperty { get; set; }

        string ReadOnlyProperty { get; }

        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Required for testing.")]
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

        int MethodWithOutputAndReference(out int argument1, ref int argument2);
    }
}
