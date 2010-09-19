using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    public class IsProxyConstraint
            : NUnit.Framework.Constraints.Constraint
    {

        public override bool Matches(object actual)
        {
            this.actual = actual;
            return actual != null && Fake.GetFakeManager(actual) != null;
        }

        public override void WriteDescriptionTo(NUnit.Framework.Constraints.MessageWriter writer)
        {
            writer.WriteExpectedValue("Proxy");
        }
    }
}
