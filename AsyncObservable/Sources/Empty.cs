using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Empty<T> : IAsyncObservable<T>
    {
        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var disposable = new BooleanDisposable();

            await observer.OnSubscribeAsync(disposable);

            if (!disposable.IsDisposed)
                await observer.OnCompletedAsync();

            await observer.OnFinallyAsync();
        }
    }
}
