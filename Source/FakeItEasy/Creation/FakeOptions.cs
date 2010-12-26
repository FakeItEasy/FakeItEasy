namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.SelfInitializedFakes;

    internal class FakeOptions
    {
        public FakeOptions()
        {
            this.AdditionalInterfacesToImplement = Enumerable.Empty<Type>();
        }
        
        public static FakeOptions Empty
        {
            get { return new FakeOptions(); }
        }

        public object WrappedInstance { get; set; }

        public ISelfInitializingFakeRecorder SelfInitializedFakeRecorder { get; set; }

        public IEnumerable<object> ArgumentsForConstructor { get; set; }

        public IEnumerable<Type> AdditionalInterfacesToImplement { get; set; }
    }
}