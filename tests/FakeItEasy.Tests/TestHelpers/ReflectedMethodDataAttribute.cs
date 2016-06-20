namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
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
    public sealed class ReflectedMethodDataAttribute : DataAttribute
    {
        private readonly string methodName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectedMethodDataAttribute"/> class.
        /// </summary>
        /// <param name="methodName">
        /// The name of the public static member on the test class that will provide the test data.
        /// </param>
        public ReflectedMethodDataAttribute(string methodName)
        {
            this.methodName = methodName;
        }

        /// <inheritdoc/>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            Guard.AgainstNull(testMethod, nameof(testMethod));

            var type = testMethod.ReflectedType;
            var obj = this.GetDataFromMethod(type);

            var dataItems = obj as IEnumerable<object>;
            if (dataItems == null)
            {
                throw new ArgumentException($"Method {methodName} on {type.FullName} did not return IEnumerable<object>");
            }

            return dataItems.Select(item => this.ConvertDataItem(type, item));
        }

        private object GetDataFromMethod(Type originalType)
        {
            MethodInfo methodInfo = null;
            for (var currentType = originalType; methodInfo == null && currentType != null; currentType = currentType.GetTypeInfo().BaseType)
            {
                methodInfo = currentType.GetRuntimeMethods()
                    .FirstOrDefault(m => m.Name == this.methodName && m.GetParameters().Length == 0);
            }

            if (methodInfo == null)
            {
                throw new ArgumentException($"Could not find public static method '{originalType.FullName}.{methodName}'");
            }

            return methodInfo.Invoke(null, new object[0]);
        }

        /// <summary>
        /// Converts an item yielded by the data method to an object array, for return from <see cref="GetData"/>.
        /// </summary>
        /// <param name="type">The type that provides the data method.</param>
        /// <param name="item">An item yielded from the data method.</param>
        /// <returns>An <see cref="T:object[]"/> suitable for returning from <see cref="GetData"/>.</returns>
        private object[] ConvertDataItem(Type type, object item)
        {
            var array = item as object[];
            if (array == null)
            {
                throw new ArgumentException($"Method {type}.{methodName}  yielded an item that is not an object[]");
            }

            return array;
        }
    }
}
