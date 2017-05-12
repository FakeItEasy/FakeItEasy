namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using Xunit.Sdk;

    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "No need to access culture name.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class UsingCultureAttribute : BeforeAfterTestAttribute
    {
        private readonly string cultureName;

        private CultureInfo originalCulture;
        private CultureInfo originalUiCulture;

        public UsingCultureAttribute(string cultureName)
        {
            this.cultureName = cultureName;
        }

        public override void After(MethodInfo methodUnderTest)
        {
#if FEATURE_THREAD_CURRENTCULTURE
            Thread.CurrentThread.CurrentCulture = this.originalCulture;
            Thread.CurrentThread.CurrentUICulture = this.originalUiCulture;
#else
            CultureInfo.CurrentCulture = this.originalCulture;
            CultureInfo.CurrentUICulture = this.originalUiCulture;
#endif
        }

        public override void Before(MethodInfo methodUnderTest)
        {
#if FEATURE_THREAD_CURRENTCULTURE
            this.originalCulture = Thread.CurrentThread.CurrentCulture;
            this.originalUiCulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(this.cultureName);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(this.cultureName);
#else
            this.originalCulture = CultureInfo.CurrentCulture;
            this.originalUiCulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentCulture = new CultureInfo(this.cultureName);
            CultureInfo.CurrentUICulture = new CultureInfo(this.cultureName);
#endif
        }
    }
}
