namespace FakeItEasy.Tests;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy.Configuration;
using FakeItEasy.Core;

using static FakeItEasy.Tests.TestHelpers.ExpressionHelper;

/// <summary>
/// A fake implementation of IFakeObjectCall, used for testing.
/// </summary>
public class FakeCall : IInterceptedFakeObjectCall, ICompletedFakeObjectCall
{
    private FakeCall(MethodInfo method, ArgumentCollection argumentCollection, object fakedObject, int sequenceNumber)
    {
        this.Method = method;
        this.Arguments = argumentCollection;
        this.FakedObject = fakedObject;
        this.SequenceNumber = sequenceNumber;
    }

    public MethodInfo Method { get; private set; }

    public ArgumentCollection Arguments { get; private set; }

#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
    public ArgumentCollection ArgumentsAfterCall => throw new NotImplementedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations

    public object? ReturnValue { get; private set; }

    public object FakedObject { get; private set; }

    public int SequenceNumber { get; private set;  }

    public static FakeCall Create<T>(Expression<Action<T>> callSpecification) where T : class
    {
        var method = GetMethodInfo(callSpecification);
        var arguments = ((MethodCallExpression)callSpecification.Body).Arguments
            .Select(ExpressionExtensions.Evaluate)
            .ToArray();

        return new FakeCall(
            method,
            new ArgumentCollection(arguments, method),
            A.Fake<T>(),
            SequenceNumberManager.GetNextSequenceNumber());
    }

    public void SetReturnValue(object? value)
    {
        this.ReturnValue = value;
    }

    public void CallBaseMethod()
    {
    }

    public void SetArgumentValue(int index, object? value)
    {
    }
}
