namespace FakeItEasy.DynamicProxy
{
    /// <summary>
    /// An interface implemented by DynamicProxy proxies in order to let them
    /// intercept object members.
    /// 
    /// NOT INTENDED FOR USE! Note that this interface WILL be removed from future versions of FakeItEasy.
    /// </summary>
    public interface ICanInterceptObjectMembers
    {
        string ToString();

        bool Equals(object o);

        int GetHashCode();
    }
}
