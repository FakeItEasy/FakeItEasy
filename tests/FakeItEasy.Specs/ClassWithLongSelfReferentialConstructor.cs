namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class ClassWithLongSelfReferentialConstructor
    {
        public readonly object NumberOfConstructorParameters;

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "This is just a dummy argument.")]
        public ClassWithLongSelfReferentialConstructor(Type type) =>
            this.NumberOfConstructorParameters = 1;

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "type", Justification = "This is just a dummy argument.")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "classWithLongSelfReferentialConstructor", Justification = "This is just a dummy argument.")]
        public ClassWithLongSelfReferentialConstructor(
            Type type,
            ClassWithLongSelfReferentialConstructor classWithLongSelfReferentialConstructor) =>
            this.NumberOfConstructorParameters = 2;
    }
}
