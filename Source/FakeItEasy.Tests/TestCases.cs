using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using FakeItEasy.Core;
using System.Reflection;

namespace FakeItEasy.Tests
{
    public static class TestCases
    {
        public static object[] AsTestCaseSource<T>(this IEnumerable<T> cases, Func<T, object[]> caseProjection)
        {
            return
                (from @case in cases
                 select caseProjection(@case)).ToArray();
        }

        public static object[] AsTestCaseSource<T>(this IEnumerable<T> cases)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            Func<T, object[]> projection = x =>
                {
                    var result = new object[properties.Length];

                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = properties[i].GetValue(x, null);
                    }

                    return result;
                };

            return cases.AsTestCaseSource(projection);
        }

        public static IEnumerable<T> Create<T>(params T[] cases)
        {
            return cases;
        }
    }
}
