namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using FakeItEasy.Core;

    internal class FakeOptions
    {
        private FakeWrapperConfigurator wrapper;

        public FakeOptions()
        {
            this.AdditionalInterfacesToImplement = Enumerable.Empty<Type>();
            this.FakeConfigurationActions = new List<Action<object>>();
            this.AdditionalAttributes = new List<CustomAttributeBuilder>();
        }
        
        public static FakeOptions Empty
        {
            get { return new FakeOptions(); }
        }

        public FakeWrapperConfigurator Wrapper
        {
            get
            {
                return this.wrapper;
            }

            set
            {
                this.wrapper = value;
                this.FakeConfigurationActions.Add(fake => this.wrapper.ConfigureFakeToWrap(fake));
            }
        }

        public IEnumerable<object> ArgumentsForConstructor { get; set; }

        public IEnumerable<Type> AdditionalInterfacesToImplement { get; set; }

        public ICollection<Action<object>> FakeConfigurationActions { get; set; }

        public ICollection<CustomAttributeBuilder> AdditionalAttributes { get; set; }
    }
}