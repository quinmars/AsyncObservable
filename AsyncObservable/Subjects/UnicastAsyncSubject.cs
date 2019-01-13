using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public class UnicastAsyncSubject<T> : IAsyncSubject<T>, IDisposable
    {
        TaskCompletionSource<Unit> _tcsSubscription;
        TaskCompletionSource<IAsyncObserver<T>> _tcsObserver;
        IAsyncObserver<T> _observer;


        public UnicastAsyncSubject()
        {
            _tcsSubscription = new TaskCompletionSource<Unit>();
            _tcsObserver = new TaskCompletionSource<IAsyncObserver<T>>();
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            if (_observer != null)
                throw new InvalidOperationException("Only one subscription is allowed!");

            _observer = observer;
            _tcsObserver.SetResult(observer);

            await _tcsSubscription.Task.ConfigureAwait(false);
        }
        
        public ValueTask OnCompletedAsync()
        {
            if (IsCanceled)
                return default;

            return _observer.OnCompletedAsync();
        }

        public ValueTask OnErrorAsync(Exception error)
        {
            if (IsCanceled)
                return default;

            return _observer.OnErrorAsync(error);
        }

        public ValueTask OnNextAsync(T value)
        {
            if (IsCanceled)
                return default;

            return _observer.OnNextAsync(value);
        }

        private IDisposable _upstream;

        public bool IsCanceled { get; private set; }

        public virtual void Dispose()
        {
            Interlocked.Exchange(ref _upstream, null)?.Dispose();
            IsCanceled = true;
        }

        public ValueTask OnSubscribeAsync(IDisposable cancelable)
        {
            _upstream = cancelable;

            if (_observer == null)
                return AsyncOnSubscribeAsync(this);

            return _observer.OnSubscribeAsync(this);
        }

        public async ValueTask AsyncOnSubscribeAsync(IDisposable cancelable)
        {
            _observer = await _tcsObserver.Task.ConfigureAwait(false);
            await _observer.OnSubscribeAsync(this).ConfigureAwait(false);
        }

        public async ValueTask OnFinallyAsync()
        {
            try
            {
                await _observer.OnFinallyAsync().ConfigureAwait(false);
            }
            finally
            {
                _tcsSubscription.SetResult(Unit.Default);
            }
        }
    }
}
