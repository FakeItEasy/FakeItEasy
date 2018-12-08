namespace FakeItEasy.Examples.ExampleObjects
{
    using System;

    public class WidgetEventArgs
        : EventArgs
    {
        public WidgetEventArgs(string widgetName)
        {
            this.WidgetName = widgetName;
        }

        public string WidgetName { get; }
    }
}
