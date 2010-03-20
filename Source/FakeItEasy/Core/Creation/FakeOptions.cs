namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    internal class FakeOptions
    {
        public IEnumerable<object> ArgumentsForConstructor { get; set; }
    }
}
