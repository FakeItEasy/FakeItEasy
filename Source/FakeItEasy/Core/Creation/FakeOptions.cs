namespace FakeItEasy.Core.Creation
{
    using System.Collections.Generic;
    using FakeItEasy.SelfInitializedFakes;

    internal class FakeOptions
    {
        public object WrappedInstance { get; set; }
        public ISelfInitializingFakeRecorder SelfInitializedFakeRecorder { get; set; }
        public IEnumerable<object> ArgumentsForConstructor { get; set; }
    }
}
