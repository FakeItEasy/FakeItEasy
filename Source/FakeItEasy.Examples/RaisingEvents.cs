namespace FakeItEasy.Examples
{
    using FakeItEasy.Examples.ExampleObjects;

    public class RaisingEvents
    {
        public void Raising_event_specifying_both_sender_and_event_arguments()
        {
            var widget = A.Fake<IWidget>();
            widget.WidgetBroke += Raise.With(widget, new WidgetEventArgs("widget name"));
        }

        public void Raising_event_specifying_event_arguments_only()
        {
            var widget = A.Fake<IWidget>();

            // When raising like this the fake object is set as sender.
            widget.WidgetBroke += Raise.With(new WidgetEventArgs("widget name"));
        }
    }
}