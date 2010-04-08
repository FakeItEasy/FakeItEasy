namespace FakeItEasy.DynamicProxy
{
    /// <summary>
    /// An interface implemented by DynamicProxy proxies in order to let them
    /// intercept object members.
    /// </summary>
    internal interface ICanInterceptObjectMembers
    {
        string ToString();

        bool Equals(object o);

        int GetHashCode();
    }
}
