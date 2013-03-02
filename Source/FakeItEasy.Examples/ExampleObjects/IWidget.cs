namespace FakeItEasy.Examples.ExampleObjects
{
    using System;

    public interface IWidget
    {
        event EventHandler<WidgetEventArgs> WidgetBroke;

        string Name { get; set; }

        void Repair();
    }
}
