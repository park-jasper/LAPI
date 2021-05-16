using System.Threading;
using System.Threading.Tasks;

namespace LAPI.Domain.Extensions
{
    public static class TaskExtensions
    {
        private static void ThrowOnError(Task task)
        {
            if (task.Status == TaskStatus.Faulted)
            {
                throw task.Exception.InnerException;
            }
        }
        public static Task WithCancellationToken(this Task task, CancellationToken token)
        {
            return task.ContinueWith(ThrowOnError, token);
        }
        public static Task<TResult> WithCancellationToken<TResult>(this Task<TResult> task, CancellationToken token)
        {
            return task.ContinueWith(t =>
            {
                ThrowOnError(t);
                return t.Result;
            }, token);
        }
    }
}