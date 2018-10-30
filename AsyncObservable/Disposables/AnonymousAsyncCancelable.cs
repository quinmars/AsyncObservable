using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class AnonymousAsyncCancelable : IAsyncCancelable
    {
        readonly TaskCompletionSource<bool> _taskCompletion = new TaskCompletionSource<bool>();

        public bool IsDisposing { get; private set; }
        public bool IsDisposed => _taskCompletion.Task.IsCompleted;

        public void SetDisposed() => _taskCompletion.SetResult(true);
        public void SetException(Exception ex) => _taskCompletion.SetException(ex);

        public ValueTask DisposeAsync()
        {
            IsDisposing = true;
            return new ValueTask(_taskCompletion.Task);
        }

        public void Dispose()
        {
            IsDisposing = true;
        }
    }
}
