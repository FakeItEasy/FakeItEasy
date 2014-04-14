namespace FakeItEasy
{
    using System.Threading.Tasks;

    internal class TaskHelper
    {
        public static Task<T> FromResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }
    }
}