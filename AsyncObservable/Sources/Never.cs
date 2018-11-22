using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Never<T> : IAsyncObservable<T>
    {
        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var disposable = new AwaitableDisposable();

            await observer.OnSubscribeAsync(disposable);

            try
            {
                // AwaitableDisposable will not throw, hence we
                // do not need to follow the try finally pattern.
                // We do it nonetheless to be consistent.
                await disposable.Task;
            }
            finally
            {
                await observer.OnFinallyAsync();
            }
        }
    }
}
