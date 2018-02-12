namespace FakeItEasy
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Threading.Tasks;
    using FakeItEasy.Compatibility;

    internal static class TaskHelper
    {
        private static readonly ConcurrentDictionary<Type, Task> CachedCanceledTasks = new ConcurrentDictionary<Type, Task>();

        private static readonly MethodInfo CreateGenericCanceledTaskGenericDefinition =
            typeof(TaskHelper).GetMethod(
                nameof(CreateGenericCanceledTask),
                BindingFlags.Static | BindingFlags.NonPublic);

        public static Task<T> FromResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }

        public static Task FromException(Exception exception)
        {
            return FromException<int>(exception);
        }

        public static Task<T> FromException<T>(Exception exception)
        {
            var source = new TaskCompletionSource<T>();
            source.SetException(exception);
            return source.Task;
        }

        public static Task Canceled()
        {
            return Canceled(typeof(int));
        }

        public static Task Canceled(Type resultType)
        {
            if (resultType == typeof(void))
            {
                return Canceled();
            }

            return CachedCanceledTasks.GetOrAdd(
                resultType,
                type =>
                {
                    var method = CreateGenericCanceledTaskGenericDefinition.MakeGenericMethod(type);
                    return (Task)method.Invoke(null, ArrayHelper.Empty<object>());
                });
        }

        private static Task<T> CreateGenericCanceledTask<T>()
        {
            var source = new TaskCompletionSource<T>();
            source.SetCanceled();
            return source.Task;
        }
    }
}
