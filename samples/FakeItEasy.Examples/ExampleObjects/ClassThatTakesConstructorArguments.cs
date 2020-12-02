namespace FakeItEasy.Examples.ExampleObjects
{
    using System;

    public class ClassThatTakesConstructorArguments
    {
#pragma warning disable CA1801 // Parameter is never used
        public ClassThatTakesConstructorArguments(IWidgetFactory foo, string name)
        {
        }
#pragma warning restore CA1801 // Parameter is never used
    }
}
