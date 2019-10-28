namespace FakeItEasy.Core
{
    /// <summary>
    /// Represents a call to a fake object at interception time.
    /// </summary>
    public interface IInterceptedFakeObjectCall : IFakeObjectCall
    {
        /// <summary>
        /// Sets the return value of the call.
        /// </summary>
        /// <param name="value">The return value to set.</param>
        void SetReturnValue(object? value);

        /// <summary>
        /// Calls the base method of the faked type.
        /// </summary>
        void CallBaseMethod();

        /// <summary>
        /// Sets the value of the argument at the specified index in the parameters list.
        /// </summary>
        /// <param name="index">The index of the argument to set the value of.</param>
        /// <param name="value">The value to set to the argument.</param>
        void SetArgumentValue(int index, object? value);
    }
}
