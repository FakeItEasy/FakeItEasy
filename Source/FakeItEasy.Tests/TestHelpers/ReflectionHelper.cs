namespace FakeItEasy.Tests.TestHelpers
{
    using System.Reflection;

    public class ReflectionHelper
    {
        public static PropertyInfo VirtualProperty
        {
            get
            {
                return typeof(Foo).GetProperty("VirtualProperty", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
        }

        
    }
}
