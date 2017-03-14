namespace FakeItEasy.Tests.TestHelpers
{
    using System.Linq;
    using System.Reflection;

    public static class FakeMethodHelper
    {
        public static MethodInfo CreateFakeMethod(string[] parameterNames)
        {
            var parameters = A.CollectionOfFake<ParameterInfo>(parameterNames.Length).ToArray();
            for (int i = 0; i < parameterNames.Length; i++)
            {
                var parameter = parameters[i];
                A.CallTo(() => parameter.Name).Returns(parameterNames[i]);
            }

            var method = A.Fake<MethodInfo>();
            A.CallTo(() => method.GetParameters()).Returns(parameters);

            return method;
        }
    }
}
