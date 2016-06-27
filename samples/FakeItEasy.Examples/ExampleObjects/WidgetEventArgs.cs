namespace FakeItEasy.Examples.ExampleObjects
{
    using System;

    [Serializable]
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
