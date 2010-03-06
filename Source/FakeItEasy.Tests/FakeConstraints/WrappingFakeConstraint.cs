using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework.Constraints;
using FakeItEasy.Core;

namespace FakeItEasy.Tests.FakeConstraints
{
    internal class WrappingFakeConstraint
        : Constraint
    {
        public override bool Matches(object actual)
        {
            this.actual = actual;

            var fake = Fake.GetFakeObject(actual);

            return fake.Rules.Any(x => x is WrappedObjectRule);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WritePredicate("A fake object wrapper.");
        }
    }
}
