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
        private readonly string testContextSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedContextDataAttribute"/> class.
        /// </summary>
        /// <param name="testContextSource">A source of test cases.</param>
        /// <param name="methodName">
        /// The name of the public static member on the test class that will provide the test data.
        /// </param>
        public TypedContextDataAttribute(string testContextSource, string methodName = null)
        {
            Guard.AgainstNull(testContextSource, nameof(testContextSource));

            this.methodName = methodName;
            this.testContextSource = testContextSource;
        }

        /// <inheritdoc/>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            Guard.AgainstNull(testMethod, nameof(testMethod));

            var testCaseSourceParams = new MemberDataAttribute(this.testContextSource).GetData(testMethod);

            foreach (object testContext in testCaseSourceParams.Select(p => p.First()))
            {
                if (testContext.GetType().GetMethod(testMethod.Name) != null)
                {
                    // the test context has its own implementation of the method being tested - skip this case
                    continue;
                }

                if (this.methodName != null)
                {
                    var dataSource = testContext.GetType().GetMethod(this.methodName);
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
