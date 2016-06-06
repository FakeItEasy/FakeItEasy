namespace FakeItEasy.Tests
{
    using System;

    public class FakeTests : FacadedTestBase
    {
        protected override Type FacadedType
        {
            get { return typeof(Fake); }
        }
    }
}
