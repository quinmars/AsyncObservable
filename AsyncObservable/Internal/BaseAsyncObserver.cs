using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    abstract class BaseAsyncObserver<TSource, TResult> : IAsyncObserver<TSource>, IDisposable
    {
        private readonly IAsyncObserver<TResult> _downstream;
        private IDisposable _upstream;

        public bool IsCanceled { get; private set; }

        public BaseAsyncObserver(IAsyncObserver<TResult> observer)
        {
            _downstream = observer;
        }

        public virtual ValueTask OnSubscribeAsync(IDisposable disposable)
        {
            _upstream = disposable;
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

        public virtual void Dispose()
        {
            IsCanceled = true;
            _upstream.Dispose();
        }

        /*
         * Helper
         */
        protected void SetUpstream(IDisposable upstream) => _upstream = upstream;
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

    class BaseAsyncObserver<T> : BaseAsyncObserver<T, T>
    {
        public BaseAsyncObserver(IAsyncObserver<T> observer) : base(observer)
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
