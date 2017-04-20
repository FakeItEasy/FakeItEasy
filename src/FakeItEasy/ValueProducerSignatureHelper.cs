namespace FakeItEasy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

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

                throw new FakeConfigurationException(
                    $"The faked method has the signature ({fakeSignature}), but {nameOfFeature} was used with ({actionSignature}).");
            }
        }

        internal static void AssertThatValuesSatisfyCallSignature(MethodInfo callMethod, object[] values)
        {
            if (IsCallSignatureSatisfiedByValues(callMethod, values))
            {
                return;
            }

            var formatter = ServiceLocator.Current.Resolve<ArgumentValueFormatter>();

            var fakeSignature = BuildSignatureDescription(callMethod.GetParameters().Select(p => (object)p.ParameterType), formatter);
            var actionSignature = BuildSignatureDescription(values.Select(v => v == null ? null : (object)v.GetType()), formatter);

            throw new FakeConfigurationException(
                $"The event has the signature ({fakeSignature}), but the provided arguments have types ({actionSignature}).");
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

        private static bool IsCallSignatureSatisfiedByValues(MethodInfo callMethod, object[] values)
        {
            var callMethodParameterTypes = callMethod.GetParameters().Select(p => p.ParameterType).ToList();

            if (callMethodParameterTypes.Count != values.Length)
            {
                return false;
            }

            for (int i = 0; i < callMethodParameterTypes.Count; i++)
            {
                if (values[i] == null)
                {
                    if (callMethodParameterTypes[i].GetTypeInfo().IsValueType)
                    {
                        return false;
                    }
                }
                else if (!callMethodParameterTypes[i].IsInstanceOfType(values[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static string BuildSignatureDescription(MethodInfo method)
        {
            return method.GetParameters().ToCollectionString(x => x.ParameterType.ToString(), ", ");
        }

        private static string BuildSignatureDescription(IEnumerable<object> types, ArgumentValueFormatter formatter)
        {
            return types.ToCollectionString(formatter.GetArgumentValueAsString, ", ");
        }
    }
}
