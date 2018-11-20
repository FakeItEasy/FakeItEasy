namespace FakeItEasy.Specs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using FakeItEasy.Sdk;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class SerializationSpecs
    {
        [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "It's just used for testing.")]
        public interface IInterface
        {
            int ParseAString(string someString);
        }

        [Scenario]
        [InlineData(typeof(IInterface))]
        [InlineData(typeof(AbstractClass))]
        [InlineData(typeof(ClassWithProtectedConstructor))]
        [InlineData(typeof(ClassWithInternalConstructorVisibleToDynamicProxy))]
        [InlineData(typeof(InternalClassVisibleToDynamicProxy))]
        public static void SerializingSupportedTypes(Type typeOfFake, object fake)
        {
            "Given a serializable fake type"
                .See(typeOfFake.ToString());

            "When I create a fake of the supported type"
                .x(() => fake = Create.Fake(typeOfFake));

            "Then the fake can be serialized and deserialized"
                .x(() => SerializeAndDeserialize(fake).Should().BeAssignableTo(typeOfFake));
        }

        // A characterization test, representing the current capabilities of the code, not the desired state.
        [Scenario]
        public static void SerializingWithCompletedCalls(IInterface fake, IInterface deserializedFake)
        {
            "Given a serializable fake"
                .x(() => fake = A.Fake<IInterface>());

            "And I call a method"
                .x(() => fake.ParseAString("a string"));

            "When I serialize and deserialize the fake"
                .x(() => deserializedFake = SerializeAndDeserialize(fake));

            "Then the record of the completed call is lost"
                .x(() => Fake.GetCalls(deserializedFake).Should().BeEmpty());
        }

        // A characterization test, representing the current capabilities of the code, not the desired state.
        [Scenario]
        public static void SerializingWithConfiguredCalls(IInterface fake, Exception exception)
        {
            "Given a serializable fake"
                .x(() => fake = A.Fake<IInterface>());

            "And I configure a method"
                .x(() => A.CallTo(() => fake.ParseAString("a string")).Returns(3));

            "When I serialize the fake"
                .x(() => exception = Record.Exception(() => Serialize(fake)));

            "Then it throws a serialization exception"
                .x(() => exception.Should().BeAnExceptionOfType<SerializationException>());
        }

        [Scenario]
        public static void UseDeserializedFake(IInterface fake, IInterface deserializedFake)
        {
            "Given a serializable fake"
                .x(() => fake = A.Fake<IInterface>());

            "When I serialize and deserialize it"
                .x(() => deserializedFake = SerializeAndDeserialize(fake));

            "Then I can use the fake"
                .x(() => deserializedFake.ParseAString("zero").Should().Be(0));
        }

        [Scenario]
        public static void ConfigureDeserializedFake(IInterface fake, IInterface deserializedFake)
        {
            "Given a serializable fake"
                .x(() => fake = A.Fake<IInterface>());

            "And I serialize and deserialize it"
                .x(() => deserializedFake = SerializeAndDeserialize(fake));

            "When I configure the deserialized fake"
                .x(() => A.CallTo(() => deserializedFake.ParseAString(A<string>._)).Returns(7));

            "Then the configuration is retained"
                .x(() => deserializedFake.ParseAString("seven").Should().Be(7));
        }

        private static T SerializeAndDeserialize<T>(T value)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        private static void Serialize<T>(T value)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);
            }
        }

        [Serializable]
        public abstract class AbstractClass
        {
        }

        [Serializable]
        public class ClassWithProtectedConstructor
        {
            protected ClassWithProtectedConstructor()
            {
            }
        }
    }
}
