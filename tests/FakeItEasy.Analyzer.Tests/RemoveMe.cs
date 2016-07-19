using System;

//Temporary class to get non-virtual test correct
namespace FakeItEasy.Analyzer.Tests
{
    class TheClass
    {
        void TheTest()
        {
            var foo = A.Fake<Foo>();
            A.CallTo(() => foo.Bar());
        }
    }

    internal class Foo
    {
        internal void Bar()
        {
            throw new NotImplementedException();
        }
    }
}
