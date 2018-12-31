using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Throw<T> : IAsyncObservable<T>
    {
        readonly Exception _exception;

        public Throw(Exception ex)
        {
            _exception = ex;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var disposable = new BooleanDisposable();

            await observer.OnSubscribeAsync(disposable).ConfigureAwait(false);

            if (!disposable.IsDisposed)
            {
                await observer.OnErrorAsync(_exception).ConfigureAwait(false);
            }

            await observer.OnFinallyAsync().ConfigureAwait(false);
        }
    }
}
