namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ResolvedConstructor
    {
        public ResolvedConstructor(
            IEnumerable<Type> parameterTypes,
            DummyCreationSession session,
            IDummyValueResolver resolver)
        {
            this.Arguments = ResolveArguments(parameterTypes, session, resolver);
        }

        public bool WasSuccessfullyResolved => this.Arguments.All(x => x.WasResolved);

        public IEnumerable<ResolvedArgument> Arguments { get; }

        public string ReasonForFailure { get; set; }

        private static IEnumerable<ResolvedArgument> ResolveArguments(
            IEnumerable<Type> parameterTypes,
            DummyCreationSession session,
            IDummyValueResolver resolver)
        {
            var resolvedArguments = new List<ResolvedArgument>();
            foreach (var parameterType in parameterTypes)
            {
                var resolvedArgument = new ResolvedArgument { ArgumentType = parameterType };
                try
                {
                    var creationResult = resolver.TryResolveDummyValue(session, parameterType);
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
