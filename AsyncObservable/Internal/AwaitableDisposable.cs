using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class AwaitableDisposable : ICancelable
    {
        readonly TaskCompletionSource<Unit> _tcs = new TaskCompletionSource<Unit>();

        public bool IsDisposed => _tcs.Task.IsCompleted;
        public Task Task => _tcs.Task;

        public void Dispose()
        {
            _tcs.SetResult(default);
        }
    }
}
