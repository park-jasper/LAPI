using System;
using System.Threading;
using System.Threading.Tasks;

namespace LAPI.Test.Mocks
{
    public static class MockAsyncResult
    {
        public static Task<TResult> ToApm<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<TResult>(state);

            task.ContinueWith(
                t =>
                {
                    if (task.IsFaulted)
                    {
                        tcs.TrySetException(task.Exception.InnerExceptions);
                    }
                    else if (task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(task.Result);
                    }

                    callback?.Invoke(tcs.Task);
                }, 
                CancellationToken.None, 
                TaskContinuationOptions.None, 
                TaskScheduler.Default);

            return tcs.Task;
        }
    }
}