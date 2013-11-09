namespace FakeItEasy.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class FakeTests : FacadedTestBase
    {
        protected override Type FacadedType
        {
            get { return typeof(Fake); }
        }
    }
}