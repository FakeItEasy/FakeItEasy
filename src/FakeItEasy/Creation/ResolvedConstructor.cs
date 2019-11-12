namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    internal class ResolvedConstructor
    {
        public ResolvedConstructor(
            IEnumerable<Type> parameterTypes,
            IDummyValueResolver resolver,
            LoopDetectingResolutionContext resolutionContext)
        {
            this.Arguments = ResolveArguments(parameterTypes, resolver, resolutionContext);
        }

        public bool WasSuccessfullyResolved => this.Arguments.All(x => x.WasResolved);

        public IEnumerable<ResolvedArgument> Arguments { get; }

        [DisallowNull]
        public string? ReasonForFailure { get; set; }

        private static IEnumerable<ResolvedArgument> ResolveArguments(
            IEnumerable<Type> parameterTypes,
            IDummyValueResolver resolver,
            LoopDetectingResolutionContext resolutionContext)
        {
            var resolvedArguments = new List<ResolvedArgument>();
            foreach (var parameterType in parameterTypes)
            {
                var resolvedArgument = new ResolvedArgument(parameterType);
                try
                {
                    var creationResult = resolver.TryResolveDummyValue(parameterType, resolutionContext);
                    resolvedArgument.WasResolved = creationResult.WasSuccessful;
                    if (creationResult.WasSuccessful)
                    {
                        resolvedArgument.ResolvedValue = creationResult.Result;
                    }
                }
                catch
                {
                    resolvedArgument.WasResolved = false;
                }

                resolvedArguments.Add(resolvedArgument);
            }

            return resolvedArguments;
        }
    }
}
