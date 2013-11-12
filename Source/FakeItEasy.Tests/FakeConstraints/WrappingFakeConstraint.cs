namespace FakeItEasy.Tests.FakeConstraints
{
    using System.Linq;
    using FakeItEasy.Core;
    using NUnit.Framework.Constraints;

    internal class WrappingFakeConstraint
        : Constraint
    {
        public override bool Matches(object actual)
        {
            this.actual = actual;

            var fake = Fake.GetFakeManager(actual);

            return fake.Rules.Any(x => x is WrappedObjectRule);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            Guard.AgainstNull(writer, "writer");

            writer.WritePredicate("A fake object wrapper.");
        }
    }
}
