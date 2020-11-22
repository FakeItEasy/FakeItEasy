namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Finds appropriate <see cref="IFakeOptionsBuilder"/>s to configure fakes.
    /// </summary>
    internal class ImplicitOptionsBuilderCatalogue
    {
        private readonly IEnumerable<IFakeOptionsBuilder> allFakeOptionsBuilders;
        private readonly ConcurrentDictionary<Type, IFakeOptionsBuilder?> cachedFakeOptionsBuilders;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplicitOptionsBuilderCatalogue" /> class.
        /// </summary>
        /// <param name="fakeOptionsBuilders">The fake options builders.</param>
        public ImplicitOptionsBuilderCatalogue(IEnumerable<IFakeOptionsBuilder> fakeOptionsBuilders)
        {
            this.allFakeOptionsBuilders = fakeOptionsBuilders.OrderByDescending(factory => factory.Priority).ToArray();
            this.cachedFakeOptionsBuilders = new ConcurrentDictionary<Type, IFakeOptionsBuilder?>();
        }

        /// <summary>
        /// Gets an implicit options builder for the specified fake type.
        /// </summary>
        /// <param name="typeOfFake">The type of the fake.</param>
        /// <returns>An options builder for the type, or <c>null</c> if no implicit options builder is registered.</returns>
        public IFakeOptionsBuilder? GetImplicitOptionsBuilder(Type typeOfFake) =>
            this.cachedFakeOptionsBuilders.GetOrAdd(
                typeOfFake,
                type => this.allFakeOptionsBuilders.FirstOrDefault(builder => builder.CanBuildOptionsForFakeOfType(type)));
    }
}
