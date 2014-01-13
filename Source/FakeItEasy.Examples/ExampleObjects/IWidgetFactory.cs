namespace FakeItEasy.Examples.ExampleObjects
{
    using System;

    public interface IWidgetFactory
    {
        IWidgetFactory SubFactory { get; set; }

        IWidget Create();

        IWidget CreateWithColor(string colorName);
    }
}
