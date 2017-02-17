namespace FakeItEasy.Core
{
    using System.Collections.Generic;

    internal static class FakeObjectHelper
    {
        public static IEqualityComparer<object> EqualityComparer { get; } = new FakeEqualityComparer();

        public static new bool Equals(object x, object y)
        {
            var fakeX = x as IFakeObject;
            if (fakeX != null)
            {
                return fakeX.Equals(y);
            }

            return object.Equals(x, y);
        }

        public static int GetHashCode(object obj)
        {
            var fakeObject = obj as IFakeObject;
            if (fakeObject != null)
            {
                return fakeObject.GetHashCode();
            }

            return obj.GetHashCode();
        }

        public static string ToString(object obj)
        {
            var fakeObject = obj as IFakeObject;
            if (fakeObject != null)
            {
                return fakeObject.ToString();
            }

            return obj.ToString();
        }

        private class FakeEqualityComparer : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                return FakeObjectHelper.Equals(x, y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                return FakeObjectHelper.GetHashCode(obj);
            }
        }
    }
}
