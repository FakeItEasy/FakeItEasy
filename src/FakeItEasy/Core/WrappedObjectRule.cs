namespace FakeItEasy.Core
{
    using System;
    using System.Reflection;
    using static FakeItEasy.ObjectMembers;

    /// <summary>
    /// A call rule that applies to any call and just delegates the
    /// call to the wrapped object.
    /// </summary>
    internal class WrappedObjectRule
        : IFakeObjectCallRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedObjectRule"/> class.
        /// Creates a new instance.
        /// </summary>
        /// <param name="wrappedInstance">
        /// The object to wrap.
        /// </param>
        public WrappedObjectRule(object wrappedInstance)
        {
            this.WrappedObject = wrappedInstance;
        }

        /// <summary>
        /// Gets the number of times this call rule is valid, if it's set
        /// to null its infinitely valid.
        /// </summary>
        public int? NumberOfTimesToCall => null;

        /// <summary>
        /// Gets the wrapped object.
        /// </summary>
        public object WrappedObject { get; }

        /// <summary>
        /// Gets whether this interceptor is applicable to the specified
        /// call, if true is returned the Apply-method of the interceptor will
        /// be called.
        /// </summary>
        /// <param name="fakeObjectCall">The call to check for applicability.</param>
        /// <returns>True if the interceptor is applicable.</returns>
        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall) => true;

        /// <summary>
        /// Applies an action to the call, might set a return value or throw
        /// an exception.
        /// </summary>
        /// <param name="fakeObjectCall">The call to apply the interceptor to.</param>
        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            if (EventCall.TryGetEventCall(fakeObjectCall, out var eventCall) && eventCall.IsEventRaiser())
            {
                throw new InvalidOperationException(ExceptionMessages.WrappingFakeCannotRaiseEvent);
            }

            fakeObjectCall.CallWrappedMethod(this.WrappedObject);
        }
    }
}
