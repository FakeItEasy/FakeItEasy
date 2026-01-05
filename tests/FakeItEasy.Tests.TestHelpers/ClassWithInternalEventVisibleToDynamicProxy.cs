namespace FakeItEasy.Tests.TestHelpers;

using System;

public class ClassWithInternalEventVisibleToDynamicProxy
{
#pragma warning disable CS0067 // The event is never used
    internal virtual event EventHandler? TheEvent;
}
