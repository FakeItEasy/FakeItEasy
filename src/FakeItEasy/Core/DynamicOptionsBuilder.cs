namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Creation;

    /// <summary>
    /// Builds fake options by invoking an appropriate <see cref="IFakeOptionsBuilder"/>.
    /// </summary>
    internal class DynamicOptionsBuilder
    {
        private readonly IEnumerable<IFakeOptionsBuilder> allFakeOptionsBuilders;
        private readonly ConcurrentDictionary<Type, IFakeOptionsBuilder> cachedFakeOptionsBuilders;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicOptionsBuilder" /> class.
        /// </summary>
        /// <param name="fakeOptionsBuilders">The fake options builders.</param>
        public DynamicOptionsBuilder(IEnumerable<IFakeOptionsBuilder> fakeOptionsBuilders)
        {
            this.allFakeOptionsBuilders = fakeOptionsBuilders.OrderByDescending(factory => factory.Priority).ToArray();
            this.cachedFakeOptionsBuilders = new ConcurrentDictionary<Type, IFakeOptionsBuilder>();
        }

        /// <summary>
        /// Applies base configuration to a fake object.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeOptions">The options to build for the fake's creation.</param>
        public void BuildOptions(Type typeOfFake, IFakeOptions fakeOptions)
        {
            var fakeOptionsBuilder = this.cachedFakeOptionsBuilders.GetOrAdd(
                typeOfFake,
                type => this.allFakeOptionsBuilders.FirstOrDefault(builder => builder.CanBuildOptionsForFakeOfType(type)));

            if (fakeOptionsBuilder != null)
            {
                fakeOptionsBuilder.BuildOptions(typeOfFake, fakeOptions);
            }
        }
    }
}
