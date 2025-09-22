using System.Threading;
using System.Threading.Tasks;

namespace Exam.Infrastructure.Threading
{
    public static class WaitHandleExtensions
    {
        public static Task WaitOneAsync(this WaitHandle handle, CancellationToken ct = default)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            RegisteredWaitHandle? rwh = null;
            rwh = ThreadPool.RegisterWaitForSingleObject(
                handle,
                (state, _) =>
                {
                    var tuple = ((TaskCompletionSource<bool> tcs, RegisteredWaitHandle? rwh))state!;
                    tuple.tcs.TrySetResult(true);
                    tuple.rwh?.Unregister(null);
                },
                state: (tcs, rwh),
                millisecondsTimeOutInterval: -1,
                executeOnlyOnce: true
            );

            if (ct.CanBeCanceled)
            {
                ct.Register(() =>
                {
                    tcs.TrySetCanceled(ct);
                    rwh?.Unregister(null);
                });
            }

            return tcs.Task;
        }
    }
}
