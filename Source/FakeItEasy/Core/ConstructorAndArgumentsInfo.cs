namespace FakeItEasy.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A meta class that holds information about the resolving of a constructor
    /// and dummy values to use for its arguments.
    /// </summary>
    public class ConstructorAndArgumentsInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorAndArgumentsInfo"/> class.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="arguments">The arguments.</param>
        public ConstructorAndArgumentsInfo(ConstructorInfo constructor, IEnumerable<ArgumentInfo> arguments)
        {
            this.Constructor = constructor;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets the resolved constructor.
        /// </summary>
        public ConstructorInfo Constructor { get; private set; }

        /// <summary>
        /// Gets the dummy arguments to use when calling this constructor.
        /// </summary>
        public IEnumerable<object> ArgumentsToUse
        {
            get
            {
                return this.Arguments.Select(x => x.ResolvedValue);
            }
        }

        /// <summary>
        /// Gets the arguments meta information.
        /// </summary>
        public IEnumerable<ArgumentInfo> Arguments { get; private set; }
    }
}
