namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Provides a data source for a data theory, with the data coming from a public static method.
    /// The target method must return the test data in something something compatible with IEnumerable&lt;object[]&gt;.
    /// Caution: the method's result is completely enumerated before any test is run.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments",
        Justification = "No need to access arguments at runtime.")]
    [DataDiscoverer("Xunit.Sdk.MemberDataDiscoverer", "xunit.core")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class TypedContextDataAttribute : DataAttribute
    {
        private readonly string methodName;
        private readonly string testContextTypeSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedContextDataAttribute"/> class.
        /// </summary>
        /// <param name="methodName">
        /// The name of the public static member on the test class that will provide the test data.
        /// </param>
        /// <param name="testContextTypeSource"></param>
        public TypedContextDataAttribute(string testContextTypeSource, string methodName=null)
        {
            Guard.AgainstNull(testContextTypeSource, nameof(testContextTypeSource));

            this.methodName = methodName;
            this.testContextTypeSource = testContextTypeSource;
        }

        /// <inheritdoc/>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            Guard.AgainstNull(testMethod, nameof(testMethod));

            var testCaseSourceParams = new MemberDataAttribute(this.testContextTypeSource).GetData(testMethod);

            foreach (Type testContextType in testCaseSourceParams.Select(p=>p.First()))
            {
                var testContext = Activator.CreateInstance(testContextType);

                if (methodName != null)
                {
                    var dataSource = testContextType.GetMethod(this.methodName);
                    foreach (
                        object[] parameterSet in (IEnumerable<object[]>)dataSource.Invoke(testContext, new object[0]))
                    {
                        yield return new[] { testContext }.Concat(parameterSet).ToArray();
                    }
                }
                else
                {
                    yield return new[] { testContext };
                }
            }
        }
    }
}
