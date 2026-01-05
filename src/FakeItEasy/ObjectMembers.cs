namespace FakeItEasy;

using System;
using System.Reflection;

internal static class ObjectMembers
{
    public static readonly MethodInfo EqualsMethod = typeof(object).GetMethod(nameof(object.Equals), new[] { typeof(object) })!;
    public static readonly MethodInfo ToStringMethod = typeof(object).GetMethod(nameof(object.ToString), Type.EmptyTypes)!;
    public static readonly MethodInfo GetHashCodeMethod = typeof(object).GetMethod(nameof(object.GetHashCode), Type.EmptyTypes)!;
}
