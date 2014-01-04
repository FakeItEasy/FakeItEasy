namespace FakeItEasy
{
    using System.Linq;
    using System.Reflection;

    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides helper methods for checking the value producer signature against call signatures.
    /// </summary>
    internal static class ValueProducerSignatureHelper
    {
        internal static void AssertThatValueProducerSignatureSatisfiesCallSignature(MethodInfo callMethod, MethodInfo valueProducerMethod, string nameOfFeature)
        {
            if (!IsCallSignatureSatisfiedByValueProducerSignature(callMethod, valueProducerMethod))
            {
                var fakeSignature = BuildSignatureDescription(callMethod);
                var actionSignature = BuildSignatureDescription(valueProducerMethod);

                throw new FakeConfigurationException("The faked method has the signature ({0}), but {2} was used with ({1}).".FormatInvariant(fakeSignature, actionSignature, nameOfFeature));
            }
        }

        private static bool IsCallSignatureSatisfiedByValueProducerSignature(MethodInfo callMethod, MethodInfo valueProducerMethod)
        {
            var callMethodParameterTypes = callMethod.GetParameters().Select(p => p.ParameterType).ToList();
            var valueProducerMethodParameterTypes = valueProducerMethod.GetParameters().Select(p => p.ParameterType).ToList();

            if (callMethodParameterTypes.Count != valueProducerMethodParameterTypes.Count)
            {
                return false;
            }

            for (int i = 0; i < callMethodParameterTypes.Count; i++)
            {
                if ((callMethodParameterTypes[i] != valueProducerMethodParameterTypes[i]) &&
                    (!callMethodParameterTypes[i].IsByRef ||
                     callMethodParameterTypes[i].GetElementType() != valueProducerMethodParameterTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static string BuildSignatureDescription(MethodInfo method)
        {
            return method.GetParameters().ToCollectionString(x => x.ParameterType.FullName, ", ");
        }
    }
}