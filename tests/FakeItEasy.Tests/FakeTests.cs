namespace FakeItEasy.Tests
{
    using System;

    public class FakeTests : FacadedTestBase
    {
        protected override Type FacadedType => typeof(Fake);
    }
}
