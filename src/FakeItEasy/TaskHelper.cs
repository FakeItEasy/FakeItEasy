namespace FakeItEasy
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Threading.Tasks;

    internal static class TaskHelper
    {
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
            return Cancelled(typeof(int));
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

        private static Task<T> CreateGenericCancelledTask<T>()
        {
            var source = new TaskCompletionSource<T>();
            source.SetCanceled();
            return source.Task;
        }
    }
}
