namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class TestCases
    {
        /// <summary>
        /// Creates a series of test cases, one per input object.
        /// Each test case will be constructed as an array of objects
        /// derived from the public properties of the input object.
        /// </summary>
        /// <typeparam name="T">The type of the input test objects.</typeparam>
        /// <param name="cases">The input test objects.</param>
        /// <returns>A sequence of test cases.</returns>
        public static IEnumerable<object[]> FromProperties<T>(params T[] cases)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            Func<T, object[]> objectToPropertiesProjection = x =>
            {
                var result = new object[properties.Length];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = properties[i].GetValue(x, null);
                }

                return result;
            };

            return cases.Select(objectToPropertiesProjection);
        }

        /// <summary>
        /// Creates a series of test cases, one per input object.
        /// Each test case will be constructed as an object array
        /// containing a single member - the input object.
        /// </summary>
        /// <param name="cases">The input test objects.</param>
        /// <returns>A sequence of test cases.</returns>
        public static IEnumerable<object[]> FromObject(params object[] cases)
        {
            return cases.Select(@case => new[] { @case });
        }

        public static IEnumerable<object[]> FromObject<T>(params T[] cases)
        {
            return cases.Select(@case => new object[] { @case });
        }
    }
}
