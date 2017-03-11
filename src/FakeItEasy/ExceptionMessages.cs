namespace FakeItEasy
{
    using FakeItEasy.Messages;

    internal static class ExceptionMessages
    {
        public static NoPlaceholderMessage ArgumentNameDoesNotExist =>
            @"The specified argument name does not exist in the ArgumentList.";

        public static NoPlaceholderMessage NonConstructorExpressionMessage =>
            @"Only expression of the type ExpressionType.New (constructor calls) are accepted.";

        public static TwoPlaceholderMessage WrongConstructorExpressionTypeMessage =>
            @"Supplied constructor is for type {0}, but must be for {1}.";

        public static NoPlaceholderMessage NowCalledDirectly =>
            @"The Now-method on the event raise is not meant to be called directly, only use it to register to an event on a fake object that you want to be raised.";

        public static NoPlaceholderMessage NumberOfOutAndRefParametersDoesNotMatchCall =>
            @"The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call.";

        public static NoPlaceholderMessage WrongNumberOfArgumentNamesMessage =>
            @"The number of argument names does not match the number of arguments.";

        public static NoPlaceholderMessage MethodMissmatchWhenPlayingBackRecording =>
            @"The method of the call did not match the method of the recorded call, the recorded sequence is no longer valid.";

        public static NoPlaceholderMessage NoMoreRecordedCalls =>
            @"All the recorded calls has been applied, the recorded sequence is no longer valid.";

        public static NoPlaceholderMessage FakeCreationExceptionDefaultMessage =>
            @"Unable to create fake object.";

        public static TwoPlaceholderMessage NotRecognizedAsAFake =>
            @"Object '{0}' of type '{1}' is not recognized as a fake object.";
    }
}
