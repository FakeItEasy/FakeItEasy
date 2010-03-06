using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FakeItEasy.Core
{
    [Serializable]
    internal class DefaultReturnValueRule
        : IFakeObjectCallRule
    {
        [NonSerialized]
        private IFakeObjectGeneratorFactory commandFactoryField;

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        public void Apply(IWritableFakeObjectCall fakeObjectCall)
        {
            object returnValue = ResolveReturnValue(fakeObjectCall);

            fakeObjectCall.SetReturnValue(returnValue);
        }

        private static object ResolveReturnValue(IWritableFakeObjectCall fakeObjectCall)
        {
            var generator = GetFakeObjectGeneratorForCall(fakeObjectCall);

            if (!generator.GenerateFakeObject())
            {
                return Helpers.GetDefaultValueOfType(fakeObjectCall.Method.ReturnType);    
            }

            return generator.GeneratedFake;
        }

        private static IFakeObjectGenerator GetFakeObjectGeneratorForCall(IFakeObjectCall call)
        {
            return FakeObjectGeneratorFactory.CreateGenerationCommand(call.Method.ReturnType, null, true);
        }

        private static IFakeObjectGeneratorFactory FakeObjectGeneratorFactory
        {
            get
            {
                return ServiceLocator.Current.Resolve<IFakeObjectGeneratorFactory>();
            }
        }

        public int? NumberOfTimesToCall
        {
            get { return null; }
        }
    }
}
