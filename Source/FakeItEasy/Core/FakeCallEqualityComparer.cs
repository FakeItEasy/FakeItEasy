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

        // NOTE (adamralph): based on http://stackoverflow.com/a/263416/49241
        public int GetHashCode(IFakeObjectCall obj)
        {
            // TODO (adamralph): we should also guard against null obj.Method and obj.Arguments
            // I think the best way is to switch to https://www.nuget.org/packages/LiteGuard.Source/
            Guard.AgainstNull(obj, "obj");

            var hash = 17;
            unchecked
            {
                hash = (hash * 23) + obj.Method.GetHashCode();
                foreach (var argument in obj.Arguments.Where(arg => arg != null))
                {
                    hash = (hash * 23) + argument.GetHashCode();
                }
            }

            return hash;
        }
    }
}