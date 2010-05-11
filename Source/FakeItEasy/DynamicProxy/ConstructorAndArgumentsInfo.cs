namespace FakeItEasy.DynamicProxy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ConstructorAndArgumentsInfo
    {
        public ConstructorInfo Constructor { get; set; }

        public IEnumerable<object> ArgumentsToUse
        {
            get
            {
                return this.Arguments.Select(x => x.ResolvedValue);
            }
        }

        public IEnumerable<ArgumentInfo> Arguments { get; set; }
    }
}
