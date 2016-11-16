namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using System.Threading;
    using System.Threading.Tasks;

    /// <content>Event rule.</content>
    public partial class FakeManager
    {
#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        private class CancellationRule : IFakeObjectCallRule
        {
            public int? NumberOfTimesToCall => null;

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall) =>
                GetCancelledTokens(fakeObjectCall).Any();

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                var returnType = fakeObjectCall.Method.ReturnType;
                if (typeof(Task).GetTypeInfo().IsAssignableFrom(returnType))
                {
                    Task task;
                    if (returnType == typeof(Task))
                    {
                        task = TaskHelper.Cancelled();
                    }
                    else
                    {
                        var taskResultType = returnType.GetTypeInfo().GetGenericArguments()[0];
                        task = TaskHelper.Cancelled(taskResultType);
                    }

                    fakeObjectCall.SetReturnValue(task);
                }
                else
                {
                    var token = GetCancelledTokens(fakeObjectCall).First();
                    token.ThrowIfCancellationRequested();
                }
            }

            private static IEnumerable<CancellationToken> GetCancelledTokens(IFakeObjectCall call)
            {
                return call.Method.GetParameters()
                    .Select((param, index) => new { param, index })
                    .Where(x => x.param.ParameterType == typeof(CancellationToken))
                    .Select(x => (CancellationToken)call.Arguments[x.index])
                    .Where(token => token.IsCancellationRequested);
            }
        }
    }
}
