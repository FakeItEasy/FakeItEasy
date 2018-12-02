namespace FakeItEasy.Core
{
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using System.Threading;
    using System.Threading.Tasks;

    /// <content>Event rule.</content>
    public partial class FakeManager
    {
        private class CancellationRule : SharedFakeObjectCallRule
        {
            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall) =>
                GetCanceledToken(fakeObjectCall).HasValue;

            public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                var returnType = fakeObjectCall.Method.ReturnType;
                if (typeof(Task).GetTypeInfo().IsAssignableFrom(returnType))
                {
                    Task task;
                    if (returnType == typeof(Task))
                    {
                        task = TaskHelper.Canceled();
                    }
                    else
                    {
                        var taskResultType = returnType.GetTypeInfo().GetGenericArguments()[0];
                        task = TaskHelper.Canceled(taskResultType);
                    }

                    fakeObjectCall.SetReturnValue(task);
                }
                else
                {
                    GetCanceledToken(fakeObjectCall)?.ThrowIfCancellationRequested();
                }
            }

            private static CancellationToken? GetCanceledToken(IFakeObjectCall call)
            {
                var parameters = call.Method.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == typeof(CancellationToken))
                    {
                        var token = (CancellationToken)call.Arguments[i];
                        if (token.IsCancellationRequested)
                        {
                            return token;
                        }
                    }
                }

                return null;
            }
        }
    }
}
