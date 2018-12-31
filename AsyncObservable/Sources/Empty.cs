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

            await observer.OnSubscribeAsync(disposable).ConfigureAwait(false);

            try
            {
                if (!disposable.IsDisposed)
                    await observer.OnCompletedAsync().ConfigureAwait(false);
            }
            finally
            {
                await observer.OnFinallyAsync().ConfigureAwait(false);
            }
        }
    }
}
