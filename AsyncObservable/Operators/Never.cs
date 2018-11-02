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
            var disposable = new Disposable(observer);

            await observer.OnSubscibeAsync(disposable);
        }

        class Disposable : ICancelable
        {
            readonly IAsyncObserver<T> _asyncDisposable;

            public Disposable(IAsyncObserver<T> asyncDisposable)
            {
                _asyncDisposable = asyncDisposable;
            }

            public bool IsDisposed { get; private set; }

            public async void Dispose()
            {
                IsDisposed = true;
                await _asyncDisposable.OnFinallyAsync();
            }
        }
    }
}
