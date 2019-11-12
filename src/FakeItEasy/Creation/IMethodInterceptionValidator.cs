namespace FakeItEasy.Creation
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    internal interface IMethodInterceptionValidator
    {
        /// <summary>
        /// Gets a value indicating whether the specified method can be intercanepted by  instance.
        /// </summary>
        /// <param name="method">The member to test.</param>
        /// <param name="callTarget">The instance the method will be called on.</param>
        /// <param name="failReason">The reason the method can not be intercepted.</param>
        /// <returns>True if the member can be intercepted.</returns>
        bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, [NotNullWhen(false)]out string? failReason);
    }
}
