namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using FakeItEasy.SelfInitializedFakes;

    internal class FakeOptions
    {
        public FakeOptions()
        {
            this.AdditionalInterfacesToImplement = Enumerable.Empty<Type>();
            this.FakeConfigurationActions = new List<Action<object>>();
            this.AdditionalAttributes = Enumerable.Empty<CustomAttributeBuilder>();
        }
        
        public static FakeOptions Empty
        {
            get { return new FakeOptions(); }
        }

        public object WrappedInstance { get; set; }

        public ISelfInitializingFakeRecorder SelfInitializedFakeRecorder { get; set; }

        public IEnumerable<object> ArgumentsForConstructor { get; set; }

        public IEnumerable<Type> AdditionalInterfacesToImplement { get; set; }

        public ICollection<Action<object>> FakeConfigurationActions { get; set; }

        public IEnumerable<CustomAttributeBuilder> AdditionalAttributes { get; set; } 
    }
}