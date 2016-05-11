namespace FakeItEasy.SelfInitializedFakes
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;

    public class MethodInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MethodInfo);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            Debug.Assert(
                (string)reader.Value == nameof(MethodInfoData.QualifiedTypename),
                "Unexpected Json property name {0}. Expected: {1}.",
                (string)reader.Value,
                nameof(MethodInfoData.QualifiedTypename));
            reader.Read();
            var typeName = (string)reader.Value;
            reader.Read();
            Debug.Assert(
                (string)reader.Value == nameof(MethodInfoData.MethodName),
                "Unexpected Json property name {0}. Expected: {1}.",
                (string)reader.Value,
                nameof(MethodInfoData.MethodName));

            reader.Read();
            var methodName = (string)reader.Value;
            reader.Read();
            Debug.Assert(
                (string)reader.Value == nameof(MethodInfoData.ParameterTypes),
                "Unexpected Json property name {0}. Expected: {1}.",
                (string)reader.Value,
                nameof(MethodInfoData.ParameterTypes));
            reader.Read();
            var parameterTypeNames = ((string)reader.Value)?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Need to advance the file read pointer past the end of current object
            reader.Read(); // EndArray
            reader.Read(); // EndObject

            var type = Type.GetType(typeName);
            var parameterTypes = parameterTypeNames == null ? new Type[0] : parameterTypeNames.Select(name => Type.GetType(name)).ToArray();
            return type.GetMethod(methodName, parameterTypes);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var methodInfo = (MethodInfo)value;
            MethodInfoData data = new MethodInfoData
            {
                QualifiedTypename = methodInfo.DeclaringType.AssemblyQualifiedName,
                MethodName = methodInfo.Name,
                ParameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType.AssemblyQualifiedName).ToArray()
            };
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(data.QualifiedTypename));
            writer.WriteValue(data.QualifiedTypename);
            writer.WritePropertyName(nameof(data.MethodName));
            writer.WriteValue(data.MethodName);
            writer.WritePropertyName(nameof(data.ParameterTypes));
            writer.WriteStartArray();
            for (int i = 0; i < data.ParameterTypes.Length; i++)
            {
                writer.WriteValue(data.ParameterTypes[i]);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        private class MethodInfoData
        {
            public string QualifiedTypename { get; set; }

            public string MethodName { get; set; }

            public string[] ParameterTypes { get; set; }
        }
    }
}
