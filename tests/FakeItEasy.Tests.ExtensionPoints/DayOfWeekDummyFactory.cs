namespace FakeItEasy.Tests.ExtensionPoints
{
    using System;

    public class DayOfWeekDummyFactory : DummyFactory<DayOfWeek>
    {
        protected override DayOfWeek Create() => DayOfWeek.Saturday;
    }
}
