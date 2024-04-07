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

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Appropriate in Try-style methods")]
        private static List<ResolvedArgument> ResolveArguments(
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
                    var dummyType = parameterType.IsByRef ? parameterType.GetElementType()! : parameterType;
                    var creationResult = resolver.TryResolveDummyValue(dummyType, resolutionContext);
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
