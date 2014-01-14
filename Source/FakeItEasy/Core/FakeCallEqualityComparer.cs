namespace FakeItEasy.Core
{
    using System.Collections.Generic;
    using System.Linq;

    internal class FakeCallEqualityComparer
        : IEqualityComparer<IFakeObjectCall>
    {
        public bool Equals(IFakeObjectCall x, IFakeObjectCall y)
        {
            Guard.AgainstNull(x, "x");
            Guard.AgainstNull(y, "y");

            return x.Method.Equals(y.Method)
                && object.ReferenceEquals(x.FakedObject, y.FakedObject)
                    && x.Arguments.SequenceEqual(y.Arguments);
        }

        public int GetHashCode(IFakeObjectCall obj)
        {
            Guard.AgainstNull(obj, "obj");

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