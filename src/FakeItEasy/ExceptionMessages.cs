namespace FakeItEasy
{
    using System;
    using FakeItEasy.Core;

    internal static class ExceptionMessages
    {
        public static string ArgumentNameDoesNotExist =>
            "The specified argument name does not exist in the ArgumentList.";

        public static string NonConstructorExpression =>
            "Only expression of the type ExpressionType.New (constructor calls) are accepted.";

        public static string NowCalledDirectly =>
            "The Now-method on the event raise is not meant to be called directly, only use it to register to an event on a fake object that you want to be raised.";

        public static string NumberOfOutAndRefParametersDoesNotMatchCall =>
            "The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call.";

        public static string WrongNumberOfArguments =>
            "The number of arguments does not match the number of parameters of the method.";

        public static string FakeCreationExceptionDefault =>
            "Unable to create fake object.";

        public static string ArgumentConstraintCannotBeNestedInArgument => "An argument constraint, such as That, Ignored, or _, cannot be nested in an argument.";

        public static string WrongConstructorExpressionType(Type actualConstructorType, Type expectedConstructorType) =>
            $"Supplied constructor is for type {actualConstructorType}, but must be for {expectedConstructorType}.";

        public static string NotRecognizedAsAFake(object proxy, Type type) =>
            $"Object '{proxy}' of type {type} is not recognized as a fake object.";

        public static string CallToUnconfiguredMethodOfStrictFake(IFakeObjectCall call)
        {
            var callFormatter = ServiceLocator.Current.Resolve<IFakeObjectCallFormatter>();
            return $"Call to unconfigured method of strict fake: {callFormatter.GetDescription(call)}.";
        }

        public static string ArgumentConstraintHasWrongType(Type constraintType, Type parameterType) =>
            $"Argument constraint is of type {constraintType}, but parameter is of type {parameterType}. No call can match this constraint.";
    }
}
