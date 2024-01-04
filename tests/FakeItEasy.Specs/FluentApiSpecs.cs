namespace FakeItEasy.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;
    using Xbehave;

    /// <summary>
    /// Check certain fluent API syntax invariants.
    /// </summary>
    public static class FluentApiSpecs
    {
        [Scenario]
        public static void AAndAnShouldHaveTheSameMembers(IEnumerable<string> aMembers, IEnumerable<string> anMembers)
        {
            "Given the members on the argument constraint-creating type A"
                .x(() => aMembers = GetMembers(typeof(A<>)));

            "And the members on the argument constraint-creating type An"
                .x(() => anMembers = GetMembers(typeof(An<>)));

            "Then they should be the same"
                .x(() => aMembers.Should().BeEquivalentTo(anMembers));
        }

        public static IEnumerable<string> GetMembers(Type type)
        {
            var properties = type.GetProperties()
                .Select(m => m.Name + '[' + string.Join(", ", m.GetIndexParameters().Select(p => p.ParameterType.ToString())) + ']');

            var methods = type.GetMethods()
                    .Select(m => m.Name + '(' + string.Join(", ", m.GetParameters().Select(p => p.ParameterType.ToString())) + ')');

            return properties.Concat(methods).OrderBy(s => s);
        }
    }
}
