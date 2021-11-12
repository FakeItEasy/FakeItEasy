namespace FakeItEasy.Core
{
    using System.Reflection;
    using FakeItEasy.Creation;
    using static FakeItEasy.ObjectMethod;

    internal static class InterceptedFakeObjectCallExtensions
    {
        private static FakeAndDummyManager DummyManager => ServiceLocator.Resolve<FakeAndDummyManager>();

        public static object? GetDefaultReturnValue(this IInterceptedFakeObjectCall call) =>
            DummyManager.TryCreateDummy(call.Method.ReturnType, new LoopDetectingResolutionContext(), out object? result)
                ? result
                : call.Method.ReturnType.GetDefaultValue();

        public static void CallWrappedMethod(this IInterceptedFakeObjectCall fakeObjectCall, object wrappedObject)
        {
            Guard.AgainstNull(fakeObjectCall);
            Guard.AgainstNull(wrappedObject);

            var parameters = fakeObjectCall.Arguments.GetUnderlyingArgumentsArray();
            object? returnValue;
            try
            {
                if (fakeObjectCall.Method.GetObjectMethod() == EqualsMethod)
                {
                    var arg = parameters[0];
                    if (ReferenceEquals(arg, fakeObjectCall.FakedObject))
                    {
                        // fake.Equals(fake) returns true
                        returnValue = true;
                    }
                    else if (ReferenceEquals(arg, wrappedObject))
                    {
                        // fake.Equals(wrappedObject) returns wrappedObject.Equals(fake)
                        // This will be false if Equals isn't overriden (reference equality)
                        // and true if Equals is overriden to implement value semantics.
                        // This approach has the benefit of keeping Equals symmetrical.
                        returnValue = wrappedObject.Equals(fakeObjectCall.FakedObject);
                    }
                    else
                    {
                        // fake.Equals(somethingElse) is delegated to the wrapped object (no special case)
                        returnValue = wrappedObject.Equals(arg);
                    }
                }
                else
                {
                    returnValue = fakeObjectCall.Method.Invoke(wrappedObject, parameters);
                }
            }
            catch (TargetInvocationException ex)
            {
                ex.InnerException?.Rethrow();
                throw;
            }

            fakeObjectCall.SetReturnValue(returnValue);
        }
    }
}
