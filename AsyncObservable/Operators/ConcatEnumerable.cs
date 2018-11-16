using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class ConcatEnumerable<T> : IAsyncObservable<T>
    {
        readonly IEnumerable<IAsyncObservable<T>> _observables;

        public ConcatEnumerable(IEnumerable<IAsyncObservable<T>> observables)
        {
            _observables = observables;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var disposable = new SerialDisposable();

            await observer.OnSubscribeAsync(disposable);

            foreach (var obs in _observables)
            {
                if (disposable.IsDisposed)
                    break;

                var inner = new Observer(observer);

                try
                {
                    await obs.SubscribeAsync(inner);
                }
                catch (Exception ex)
                {
                    await observer.OnErrorAsync(ex);
                    disposable.Dispose();
                    break;
                }

                if (inner.Faulted)
                {
                    disposable.Dispose();
                    break;
                }
            }

            if (!disposable.IsDisposed)
                await observer.OnCompletedAsync();

            await observer.OnFinallyAsync();
        }

        class Observer : IAsyncObserver<T>, ICancelable
        {
            readonly IAsyncObserver<T> _downstream;
            IDisposable _upstream;

            public bool IsDisposed { get; set; }
            public bool Faulted { get; set; }

            public Observer(IAsyncObserver<T> observer)
            {
                _downstream = observer;
            }

            public ValueTask OnSubscribeAsync(IDisposable cancelable)
            {
                _upstream = cancelable;
                return default;
            }

            public ValueTask OnNextAsync(T value)
            {
                if (IsDisposed)
                    return default;

                return _downstream.OnNextAsync(value);
            }

            public ValueTask OnErrorAsync(Exception error)
            {
                if (IsDisposed)
                    return default;

                Faulted = true;
                return _downstream.OnErrorAsync(error);
            }

            public ValueTask OnCompletedAsync() => default;
            public ValueTask OnFinallyAsync() => default;

            public void Dispose()
            {
                Interlocked.Exchange(ref _upstream, null)?.Dispose();
                IsDisposed = true;
            }

        }
    }
}
