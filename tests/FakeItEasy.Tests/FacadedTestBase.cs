namespace FakeItEasy.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public abstract class FacadedTestBase
    {
        protected abstract Type FacadedType { get; }

        private Type FacadeType =>
            Type.GetType(this.FacadedType.FullName + "Facade, " + this.FacadedType.GetTypeInformation().Assembly.FullName, true);

        [Fact]
        public void The_facade_class_should_contain_instance_methods_mirroring_the_static_methods_of_the_facaded_class()
        {
            var nonMirroredMethods =
                from facadedMethod in this.FacadedType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                where !this.IsExcluedMethod(facadedMethod)
                where !(from facadeMethod in this.FacadeType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                       where this.NameSignatureAndReturnTypeEquals(facadedMethod, facadeMethod)
                       select facadeMethod).Any()
                select facadedMethod;

            nonMirroredMethods.Should().BeEmpty("some methods where not present or did not have the same signature in the facade class.");
        }

        private bool IsExcluedMethod(MethodInfo facadedMethod)
        {
            return facadedMethod.Name.Equals("ReferenceEquals") || facadedMethod.Name.Equals("Equals");
        }

        private bool NameSignatureAndReturnTypeEquals(MethodInfo method1, MethodInfo method2)
        {
            return
                object.Equals(method1.ReturnType, method2.ReturnType)
                && string.Equals(method1.Name, method2.Name)
                && this.AllArgumentsEquals(method1, method2);
        }

        private bool AllArgumentsEquals(MethodInfo method1, MethodInfo method2)
        {
            if (method1.GetParameters().Length != method2.GetParameters().Length)
            {
                return false;
            }

            foreach (var argument in method1.GetParameters()
                .Zip(
                    method2.GetParameters(),
                    (method1Parameter, method2Parameter) =>
                        new
                        {
                            Method1Parameter = method1Parameter,
                            Method2Parameter = method2Parameter
                        }))
            {
                if (!object.Equals(argument.Method1Parameter.ParameterType, argument.Method2Parameter.ParameterType) ||
                    !string.Equals(argument.Method1Parameter.Name, argument.Method2Parameter.Name))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
