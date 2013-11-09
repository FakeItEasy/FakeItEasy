namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class CallExpressionParserTests
    {
        private CallExpressionParser parser;

        private delegate int IntFunction(string argument1, object argument2);

        [SetUp]
        public void Setup()
        {
            this.parser = new CallExpressionParser();
        }

        [Test]
        public void Should_return_parsed_expression_with_instance_method_set()
        {
            // Arrange
            var call = Call(() => string.Empty.Equals(null));

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.CalledMethod, Is.EqualTo(GetMethod<string>("Equals", typeof(string))));
        }

        [Test]
        public void Should_return_parsed_expression_with_property_getter_method_set()
        {
            // Arrange
            var call = Call(() => string.Empty.Length);

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.CalledMethod, Is.EqualTo(GetPropertyGetter<string>("Length")));
        }

        [Test]
        public void Should_return_parsed_expression_with_property_getter_method_set_when_property_is_internal()
        {
            // Arrange
            var fake = A.Fake<TypeWithInternalProperty>();
            var call = Call(() => fake.InternalProperty);

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.CalledMethod, Is.EqualTo(GetPropertyGetter<TypeWithInternalProperty>("InternalProperty")));
        }

        [Test]
        public void Should_return_parsed_expression_with_static_method_set()
        {
            // Arrange
            var call = Call(() => object.Equals(null, null));

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.CalledMethod, Is.EqualTo(GetMethod<object>("Equals", typeof(object), typeof(object))));
        }

        [Test]
        public void Should_return_parsed_expression_with_target_instance_set_when_calling_method()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var call = Call(() => foo.Bar());

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.CallTarget, Is.EqualTo(foo));
        }

        [Test]
        public void Should_return_parsed_expression_with_target_instance_set_when_calling_property_getter()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var call = Call(() => foo.ChildFoo);

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.CallTarget, Is.EqualTo(foo));
        }

        [Test]
        public void Should_return_parsed_expression_with_arguments_set_when_calling_instance_method()
        {
            // Arrange
            var argumentOne = new object();
            var argumentTwo = new object();

            var foo = A.Fake<IFoo>();
            var call = Call(() => foo.Bar(argumentOne, argumentTwo));

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.Arguments, Is.EquivalentTo(new[] { argumentOne, argumentTwo }));
        }

        [Test]
        public void Should_return_parsed_expression_with_arguments_set_when_calling_indexed_property_getter()
        {
            // Arrange
            var foo = A.Fake<IList<string>>();
            var call = Call(() => foo[10]);

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.Arguments.Single(), Is.EqualTo(10));
        }

        [Test]
        public void Should_return_parsed_expression_with_argument_names_set_when_calling_instance_method()
        {
            // Arrange
            var argumentOne = new object();
            var argumentTwo = new object();

            var foo = A.Fake<IFoo>();
            var call = Call(() => foo.Bar(argumentOne, argumentTwo));

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.ArgumentsExpressions.Select(x => x.ArgumentInformation.Name), Is.EquivalentTo(new[] { "argument", "argument2" }));
        }

        [Test]
        public void Should_return_parsed_expression_with_argument_names_set_when_calling_indexed_property_getter()
        {
            // Arrange
            var foo = A.Fake<IList<string>>();
            var call = Call(() => foo[10]);

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.ArgumentsExpressions.Select(x => x.ArgumentInformation.Name).Single(), Is.EqualTo("index"));
        }

        [Test]
        public void Should_throw_when_expression_is_not_method_or_property_getter_or_invocation()
        {
            // Arrange
            var instance = new TypeWithPublicField();
            var call = Call(() => instance.PublicField);

            // Act

            // Assert
            Assert.That(
                () => this.parser.Parse(call),
                Throws.ArgumentException.With.Message.EqualTo("The specified expression is not a method call or property getter."));
        }

        [Test]
        public void Should_parse_invocation_expression_correctly()
        {
            // Arrange
            var d = new IntFunction((x, y) => 10);

            var call = Call(() => d("foo", "bar"));

            // Act
            var result = this.parser.Parse(call);

            // Assert
            Assert.That(result.CalledMethod.Name, Is.EqualTo("Invoke"));
            Assert.That(result.Arguments, Is.EquivalentTo(new object[] { "foo", "bar" }));
            Assert.That(result.ArgumentsExpressions.Select(x => x.ArgumentInformation.Name), Is.EquivalentTo(new[] { "argument1", "argument2" }));
            Assert.That(result.CallTarget, Is.SameAs(d));
        }

        private static MethodInfo GetMethod<T>(string methodName, params Type[] argumentTypes)
        {
            return typeof(T).GetMethod(methodName, argumentTypes);
        }

        private static MethodInfo GetPropertyGetter<T>(string propertyName)
        {
            return typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.Name.Equals(propertyName))
                .Select(x => x.GetGetMethod(true))
                .Single();
        }

        private static LambdaExpression Call(Expression<Action> callExpression)
        {
            return callExpression;
        }

        private static LambdaExpression Call<T>(Expression<Func<T>> callExpression)
        {
            return callExpression;
        }

        private class TypeWithPublicField
        {
            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for testing.")]
            public int PublicField = 1;
        }
    }
}