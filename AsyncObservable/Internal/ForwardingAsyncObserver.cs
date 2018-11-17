using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    abstract class ForwardingAsyncObserver<TSource, TResult> : AsyncObserverBase, IAsyncObserver<TSource>
    {
        private readonly IAsyncObserver<TResult> _downstream;

        public ForwardingAsyncObserver(IAsyncObserver<TResult> observer)
        {
            _downstream = observer;
        }

        public virtual ValueTask OnSubscribeAsync(IDisposable disposable)
        {
            SetUpstream(disposable);
            return _downstream.OnSubscribeAsync(this);
        }

        public abstract ValueTask OnNextAsync(TSource value);

        public virtual ValueTask OnErrorAsync(Exception error)
        {
            if (IsCanceled)
                return default;

            return _downstream.OnErrorAsync(error);
        }

        public virtual ValueTask OnCompletedAsync()
        {
            if (IsCanceled)
                return default;

            return _downstream.OnCompletedAsync();
        }

        public virtual ValueTask OnFinallyAsync()
        {
            return _downstream.OnFinallyAsync();
        }

        /*
         * Helper
         */
        protected ValueTask ForwardFSubscribeAsync(IDisposable d) => _downstream.OnSubscribeAsync(d);
        protected ValueTask ForwardNextAsync(TResult v) => _downstream.OnNextAsync(v);
        protected ValueTask ForwardErrorAsync(Exception ex) => _downstream.OnErrorAsync(ex);
        protected ValueTask ForwardCompletedAsync() => _downstream.OnCompletedAsync();
        protected ValueTask ForwardFinallyAsync() => _downstream.OnFinallyAsync();

        protected ValueTask SignalErrorAsync(Exception ex)
        {
            Dispose();
            return _downstream.OnErrorAsync(ex);
        }

        protected ValueTask SignalCompletedAsync()
        {
            Dispose();
            return _downstream.OnCompletedAsync();
        }
    }

    class ForwardingAsyncObserver<T> : ForwardingAsyncObserver<T, T>
    {
        public ForwardingAsyncObserver(IAsyncObserver<T> observer) : base(observer)
        {
        }

        public override ValueTask OnNextAsync(T value)
        {
            if (IsCanceled)
                return default;

            return ForwardNextAsync(value);
        }
    }
}
