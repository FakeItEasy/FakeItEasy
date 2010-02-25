using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ExpectationExceptionTests
        : ExceptionContractTests<ExpectationException>
    {
        protected override ExpectationException CreateException()
        {
            return new ExpectationException("fo");
        }
    }
}
