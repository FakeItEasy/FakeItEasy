namespace FakeItEasy
{
    using FakeItEasy.Messages;

    internal static class ExceptionMessages
    {
        public static NoPlaceholderMessage ApplicatorNotSetExceptionMessage =>
            @"The Apply method of the ExpressionInterceptor may no be called before the Applicator property has been set.";

        public static NoPlaceholderMessage ArgumentNameDoesNotExist =>
            @"The specified argument name does not exist in the ArgumentList.";

        public static NoPlaceholderMessage ArgumentsForConstructorOnInterfaceType =>
            @"Arguments for constructor was specified when generating proxy of interface type.";

        public static NoPlaceholderMessage ArgumentValidationDefaultMessage =>
            @"An argument validation was not configured correctly.";

        public static ThreePlaceholderMessage CalledTooFewTimesMessage =>
            @"The method '{0}' was called too few times, expected #{1} times but was called #{2} times.";

        public static ThreePlaceholderMessage CalledTooManyTimesMessage =>
            @"The method '{0}' was called too many times, expected #{1} times but was called #{2} times.";

        public static TwoPlaceholderMessage CanNotGenerateFakeMessage =>
            @"Can not create fake of the type '{0}', it's not registered in the current container and the current IProxyGenerator can not generate the fake.

The following constructors failed:
{1}";

        public static OnePlaceholderMessage ConfiguringNonFakeObjectExceptionMessage =>
            @"Error when accessing FakeObject, the specified argument is of the type '{0}' which is not faked.";

        public static NoPlaceholderMessage CreatingExpressionCallMatcherWithNonMethodOrPropertyExpression =>
            @"An ExpressionCallMatcher can only be created for expressions that represents a method call or a property getter.";

        public static NoPlaceholderMessage FakingNonAbstractClassWithArgumentsForConstructor =>
            @"Only abstract classes can be faked using the A.Fake-method that takes an enumerable of objects as arguments for constructor, use the overload that takes an expression instead.";

        public static NoPlaceholderMessage MemberAccessorNotCorrectExpressionType =>
            @"The member accessor expression must be a lambda expression with a MethodCallExpression or MemberAccessExpression as its body.";

        public static OnePlaceholderMessage NoConstructorMatchingArguments =>
            @"No constructor matching the specified arguments was found on the type {0}.";

        public static NoPlaceholderMessage NoDefaultConstructorMessage =>
            @"Can not generate fake object for the class since no usable default constructor was found, specify a constructor call.";

        public static NoPlaceholderMessage NonConstructorExpressionMessage =>
            @"Only expression of the type ExpressionType.New (constructor calls) are accepted.";

        public static TwoPlaceholderMessage WrongConstructorExpressionTypeMessage =>
            @"Supplied constructor is for type {0}, but must be for {1}.";

        public static NoPlaceholderMessage NowCalledDirectly =>
            @"The Now-method on the event raise is not meant to be called directly, only use it to register to an event on a fake object that you want to be raised.";

        public static NoPlaceholderMessage NumberOfOutAndRefParametersDoesNotMatchCall =>
            @"The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call.";

        public static OnePlaceholderMessage TypeCanNotBeProxied =>
            @"The current fake proxy generator can not create proxies of the type {0}.";

        public static ThreePlaceholderMessage WasCalledWrongNumberOfTimes =>
            @"Expected to find call {0} the number of times specified by the predicate '{1}' but found it {2} times among the calls:";

        public static NoPlaceholderMessage WrongNumberOfArgumentNamesMessage =>
            @"The number of argument names does not match the number of arguments.";

        public static NoPlaceholderMessage MethodMissmatchWhenPlayingBackRecording =>
            @"The method of the call did not match the method of the recorded call, the recorded sequence is no longer valid.";

        public static NoPlaceholderMessage NoMoreRecordedCalls =>
            @"All the recorded calls has been applied, the recorded sequence is no longer valid.";

        public static NoPlaceholderMessage MemberCanNotBeIntercepted =>
            @"The specified method can not be configured since it can not be intercepted by the current IProxyGenerator.";

        public static NoPlaceholderMessage FakeCreationExceptionDefaultMessage =>
            @"Unable to create fake object.";

        public static NoPlaceholderMessage SpecifiedCallIsNotToFakedObject =>
            @"The specified call is not made on a fake object.";

        public static OnePlaceholderMessage FailedToGenerateFakeWithArgumentsForConstructorPattern =>
            @"

  The current proxy generator failed to create a proxy with the specified arguments for the constructor:

  Reason for failure:
    - {0}
  ";

        public static NoPlaceholderMessage FakeManagerWasInitializedWithDifferentProxyMessage =>
            @"The fake manager was initialized for a different proxy.";

        public static TwoPlaceholderMessage NotRecognizedAsAFake =>
            @"Object '{0}' of type '{1}' is not recognized as a fake object.";
    }
}
