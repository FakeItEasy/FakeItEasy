namespace FakeItEasy.Core
{
    using System.Collections.Generic;
    using System.Linq;

    internal class FakeCallEqualityComparer
        : IEqualityComparer<IFakeObjectCall>
    {
        public bool Equals(IFakeObjectCall x, IFakeObjectCall y)
        {
            return x.Method.Equals(y.Method)
                && x.FakedObject.Equals(y.FakedObject)
                    && x.Arguments.SequenceEqual(y.Arguments);
        }

        public int GetHashCode(IFakeObjectCall obj)
        {
            var result = obj.Method.GetHashCode()
                ^ obj.FakedObject.GetHashCode();

            foreach (var argument in obj.Arguments)
            {
                result = argument != null ? result ^ argument.GetHashCode() : 0;
            }

            return result;
        }
    }
}