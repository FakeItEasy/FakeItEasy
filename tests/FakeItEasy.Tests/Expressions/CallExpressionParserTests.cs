namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class CallExpressionParserTests
    {
        private readonly CallExpressionParser parser;

        public CallExpressionParserTests()
        {
            this.parser = new CallExpressionParser();
        }

        private delegate int IntFunction(string argument1, object argument2);

        [Fact]
        public void Should_return_parsed_expression_with_instance_method_set()
        {
            // Arrange
            var call = Call(() => string.Empty.Equals(null, StringComparison.CurrentCulture));

            // Act
            var result = this.parser.Parse(call);

            // Assert
            result.CalledMethod.Should().BeSameAs(GetMethod<string>("Equals", typeof(string), typeof(StringComparison)));
        }

        [Fact]
        public void Should_return_parsed_expression_with_property_getter_method_set()
        {
            // Arrange
            var call = Call(() => string.Empty.Length);

            // Act
            var result = this.parser.Parse(call);

            // Assert
            result.CalledMethod.Should().BeSameAs(GetPropertyGetter<string>("Length"));
        }

        [Fact]
        public void Should_return_parsed_expression_with_property_getter_method_set_when_property_is_internal()
        {
            // Arrange
            var fake = A.Fake<TypeWithInternalProperty>();
            var call = Call(() => fake.InternalProperty);

            // Act
            var result = this.parser.Parse(call);

            // Assert
            result.CalledMethod.Should().BeSameAs(GetPropertyGetter<TypeWithInternalProperty>("InternalProperty"));
        }

        [Fact]
        public void Should_return_parsed_expression_with_static_method_set()
        {
            // Arrange
            var call = Call(() => object.Equals(null, null));

            // Act
            var result = this.parser.Parse(call);

            // Assert
            result.CalledMethod.Should().BeSameAs(GetMethod<object>("Equals", typeof(object), typeof(object)));
        }

        [Fact]
        public void Should_return_parsed_expression_with_target_instance_set_when_calling_method()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var call = Call(() => foo.Bar());

            // Act
            var result = this.parser.Parse(call);

            // Assert
            result.CallTarget.Should().Be(foo);
        }

        [Fact]
        public void Should_return_parsed_expression_with_target_instance_set_when_calling_property_getter()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var call = Call(() => foo.ChildFoo);

            // Act
            var result = this.parser.Parse(call);

            // Assert
            result.CallTarget.Should().Be(foo);
        }

        [Fact]
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
            result.ArgumentsExpressions.Select(x => x.ArgumentInformation.Name).Should().BeEquivalentTo("argument", "argument2");
        }

        [Fact]
        public void Should_return_parsed_expression_with_argument_names_set_when_calling_indexed_property_getter()
        {
            // Arrange
            var foo = A.Fake<IList<string>>();
            var call = Call(() => foo[10]);

            // Act
            var result = this.parser.Parse(call);

            // Assert
            result.ArgumentsExpressions.Select(x => x.ArgumentInformation.Name).Single().Should().Be("index");
        }

        [Fact]
        public void Should_throw_when_expression_is_not_method_or_property_getter_or_invocation()
        {
            // Arrange
            var instance = new TypeWithPublicField();
            var call = Call(() => instance.PublicField);

            // Act
            var exception = Record.Exception(() => this.parser.Parse(call));

            // Assert
            exception.Should()
                .BeAnExceptionOfType<ArgumentException>()
                .WithMessage("The specified expression is not a method call or property getter.");
        }

        [Fact]
        public void Should_parse_invocation_expression_correctly()
        {
            // Arrange
            var d = new IntFunction((x, y) => 10);

            var call = Call(() => d("foo", "bar"));

            // Act
            var result = this.parser.Parse(call);

            // Assert
            result.CalledMethod.Name.Should().Be("Invoke");
            result.ArgumentsExpressions.Select(x => x.ArgumentInformation.Name).Should().BeEquivalentTo("argument1", "argument2");
            result.CallTarget.Should().BeSameAs(d);
        }

        private static MethodInfo GetMethod<T>(string methodName, params Type[] argumentTypes)
        {
            return typeof(T).GetMethod(methodName, argumentTypes);
        }

        private static MethodInfo GetPropertyGetter<T>(string propertyName)
        {
            return typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.Name == propertyName)
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
