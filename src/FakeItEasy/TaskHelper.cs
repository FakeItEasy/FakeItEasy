namespace FakeItEasy
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Threading.Tasks;

    internal static class TaskHelper
    {
        private static readonly Lazy<Task> CachedCancelledTask = new Lazy<Task>(CreateGenericCancelledTask<int>);

        private static readonly ConcurrentDictionary<Type, Task> CachedCancelledTasks = new ConcurrentDictionary<Type, Task>();

        private static readonly MethodInfo CreateGenericCancelledTaskGenericDefinition =
            typeof(TaskHelper).GetMethod(
                nameof(CreateGenericCancelledTask),
                BindingFlags.Static | BindingFlags.NonPublic);

        public static Task<T> FromResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }

        public static Task Cancelled()
        {
            return CachedCancelledTask.Value;
        }

        public static Task Cancelled(Type resultType)
        {
            if (resultType == typeof(void))
            {
                return Cancelled();
            }

            return CachedCancelledTasks.GetOrAdd(
                resultType,
                type =>
                {
                    var method = CreateGenericCancelledTaskGenericDefinition.MakeGenericMethod(type);
                    return (Task)method.Invoke(null, new object[0]);
                });
        }

        public static Task Cancelled<T>()
        {
            return Cancelled(typeof(T));
        }

        private static Task<T> CreateGenericCancelledTask<T>()
        {
            var source = new TaskCompletionSource<T>();
            source.TrySetCanceled();
            return source.Task;
        }
    }
}
