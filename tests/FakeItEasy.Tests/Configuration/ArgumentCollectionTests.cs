namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ArgumentCollectionTests
    {
        [Fact]
        public void Constructor_is_properly_guarded()
        {
            Expression<Action> call = () => new ArgumentCollection(new object[] { "foo", 1 }, new[] { "foo", "bar" });
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Constructor_that_takes_method_is_properly_null_guarded()
        {
            var method = typeof(IFoo).GetMethod("Bar", new[] { typeof(object), typeof(object) });
            Expression<Action> call = () => new ArgumentCollection(new object[] { "foo", 1 }, method);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Constructor_that_takes_method_should_set_argument_names()
        {
            var method = typeof(IFoo).GetMethod("Bar", new[] { typeof(object), typeof(object) });

            var arguments = new ArgumentCollection(new object[] { "foo", "bar" }, method);

            arguments.ArgumentNames.Should().Equal("argument", "argument2");
        }

        [Fact]
        public void Constructor_should_throw_when_number_of_arguments_does_not_match_number_of_argument_names()
        {
            // act
            var exception = Record.Exception(() => new ArgumentCollection(new object[] { 1, 2 }, new[] { "first", "second", "third" }));

            // assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void Get_called_with_index_should_return_argument_typed()
        {
            var arguments = this.CreateFakeArgumentList("foo", "bar");

            arguments.Get<string>(1).Should().Be("bar");
        }

        [Fact]
        public void Get_called_with_name_should_return_argument_at_position_of_name()
        {
            var arguments = this.CreateFakeArgumentList(new[] { "foo", "bar" }, 1, 2);

            arguments.Get<int>("bar").Should().Be(2);
        }

        [Fact]
        public void Get_called_with_argument_name_that_does_not_exist_throws_exception()
        {
            var arguments = this.CreateFakeArgumentList(new[] { "foo", "bar" }, 1, 2);

            var exception = Record.Exception(() => arguments.Get<int>("unknown"));

            exception.Should().BeAnExceptionOfType<ArgumentException>();
        }

        [Fact]
        public void Count_should_return_the_number_of_arguments_in_the_collection()
        {
            var arguments = this.CreateFakeArgumentList(new[] { "foo", "bar" }, 1, 2);

            arguments.Count.Should().Be(2);
        }

        [Fact]
        public void GetEnumerator_through_non_generic_interface_returns_enumerator_that_enumerates_all_arguments()
        {
            var arguments = this.CreateFakeArgumentList(new[] { "foo", "bar" }, 1, 2);
            var found = new List<object>();

            var enumerator = ((IEnumerable)arguments).GetEnumerator();
            while (enumerator.MoveNext())
            {
                found.Add(enumerator.Current);
            }

            found.Should().Equal(1, 2);
        }

        [Fact]
        public void GetEnumerator_returns_enumerable_that_enumerates_the_arguments_of_the_collection()
        {
            var arguments = this.CreateFakeArgumentList(1, 2, 3);

            arguments.Should().Equal(1, 2, 3);
        }

        [Fact]
        public void Should_be_serializable_when_arguments_are_equatable()
        {
            // Arrange
            object collection = new ArgumentCollection(
                new object[] { "first argument" },
                typeof(string).GetMethod("Equals", new[] { typeof(string) }));

            // Act

            // Assert
            collection.Should().BeBinarySerializable();
        }

        private ArgumentCollection CreateFakeArgumentList(IEnumerable<string> argumentNames, params object[] arguments)
        {
            return new ArgumentCollection(arguments, argumentNames);
        }

        private ArgumentCollection CreateFakeArgumentList(params object[] arguments)
        {
            return this.CreateFakeArgumentList(
                Enumerable.Range(0, arguments.Length).Select(x => x.ToString(CultureInfo.CurrentCulture)).ToArray(),
                arguments);
        }
    }
}
