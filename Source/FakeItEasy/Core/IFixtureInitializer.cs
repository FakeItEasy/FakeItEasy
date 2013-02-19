namespace FakeItEasy.Core
{
    using System.Collections.Generic;

    internal interface IFixtureInitializer
    {
        void InitializeFakes(object fixture);
    }
}