namespace FakeItEasy.Examples.ExampleObjects
{
    using System;

    public interface IWidget
    {
        event EventHandler<WidgetEventArgs> WidgetBroke;

        event EventHandler WidgetRunning;

        string Name { get; set; }

        void Repair();
    }
}
