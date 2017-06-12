namespace FakeItEasy.Core
{
    using System.Reflection;

    /// <summary>
    /// A call rule that applies to any call and just delegates the
    /// call to the wrapped object.
    /// </summary>
    internal class WrappedObjectRule
        : IFakeObjectCallRule
    {
        private readonly object wrappedObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedObjectRule"/> class.
        /// Creates a new instance.
        /// </summary>
        /// <param name="wrappedInstance">
        /// The object to wrap.
        /// </param>
        public WrappedObjectRule(object wrappedInstance)
        {
            this.wrappedObject = wrappedInstance;
        }

        /// <summary>
        /// Gets the number of times this call rule is valid, if it's set
        /// to null its infinitely valid.
        /// </summary>
        public int? NumberOfTimesToCall => null;

        /// <summary>
        /// Gets whether this interceptor is applicable to the specified
        /// call, if true is returned the Apply-method of the interceptor will
        /// be called.
        /// </summary>
        /// <param name="fakeObjectCall">The call to check for applicability.</param>
        /// <returns>True if the interceptor is applicable.</returns>
        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        /// <summary>
        /// Applies an action to the call, might set a return value or throw
        /// an exception.
        /// </summary>
        /// <param name="fakeObjectCall">The call to apply the interceptor to.</param>
        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

            var parameters = fakeObjectCall.Arguments.GetUnderlyingArgumentsArray();
            object valueFromWrappedInstance;
            try
            {
                valueFromWrappedInstance = fakeObjectCall.Method.Invoke(this.wrappedObject, parameters);
            }
            catch (TargetInvocationException ex)
            {
                ex.InnerException?.Rethrow();
                throw;
            }

            fakeObjectCall.SetReturnValue(valueFromWrappedInstance);
        }
    }
}
