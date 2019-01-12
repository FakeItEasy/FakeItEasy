namespace FakeItEasy.Tests.TestHelpers
{
    using System;

    public class ClassWithInternalEventVisibleToDynamicProxy
    {
        internal virtual event EventHandler TheEvent;
    }
}
