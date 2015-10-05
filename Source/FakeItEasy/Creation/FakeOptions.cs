namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection.Emit;

    internal class FakeOptions
    {
        private readonly List<Type> additionalInterfacesToImplement = new List<Type>();
        private readonly List<Action<object>> fakeConfigurationActions = new List<Action<object>>();
        private readonly List<CustomAttributeBuilder> additionalAttributes = new List<CustomAttributeBuilder>();

        public IEnumerable<object> ArgumentsForConstructor { get; set; }

        public IEnumerable<Type> AdditionalInterfacesToImplement
        {
            get { return this.additionalInterfacesToImplement; }
        }

        public IEnumerable<Action<object>> FakeConfigurationActions
        {
            get { return this.fakeConfigurationActions; }
        }

        public IEnumerable<CustomAttributeBuilder> AdditionalAttributes
        {
            get { return this.additionalAttributes; }
        }

        public void AddInterfaceToImplement(Type interfaceType)
        {
            Guard.AgainstNull(interfaceType, "interfaceType");

            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "The specified type '{0}' is not an interface",
                        interfaceType.FullNameCSharp()));
            }

            this.additionalInterfacesToImplement.Add(interfaceType);
        }

        public void AddFakeConfigurationAction(Action<object> action)
        {
            this.fakeConfigurationActions.Add(action);
        }

        public void AddAttribute(CustomAttributeBuilder attribute)
        {
            this.additionalAttributes.Add(attribute);
        }
    }
}