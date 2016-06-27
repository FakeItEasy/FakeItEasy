namespace FakeItEasy.Core
{
    using System.Collections.Generic;
    using System.Linq;

    internal class FakeCallEqualityComparer
        : IEqualityComparer<IFakeObjectCall>
    {
        public bool Equals(IFakeObjectCall x, IFakeObjectCall y)
        {
            Guard.AgainstNull(x, nameof(x));
            Guard.AgainstNull(y, nameof(y));

            return x.Method.Equals(y.Method)
                && object.ReferenceEquals(x.FakedObject, y.FakedObject)
                    && x.Arguments.SequenceEqual(y.Arguments);
        }

        // NOTE (adamralph): based on http://stackoverflow.com/a/263416/49241
        public int GetHashCode(IFakeObjectCall obj)
        {
            Guard.AgainstNull(obj, nameof(obj));

            var hash = 17;
            unchecked
            {
                hash = (hash * 23) + (obj.Method == null ? 0 : obj.Method.GetHashCode());

                if (obj.Arguments != null)
                {
                    foreach (var argument in obj.Arguments)
                    {
                        hash = (hash * 23) + (argument == null ? 0 : argument.GetHashCode());
                    }
                }
            }

            return hash;
        }
    }
}
