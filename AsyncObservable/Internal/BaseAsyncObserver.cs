using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    abstract class BaseAsyncObserver<TSource, TResult> : IAsyncObserver<TSource>, ICancelable
    {
        protected readonly IAsyncObserver<TResult> _downstream;
        protected ICancelable _upstream;

        public bool IsDisposed { get; protected set; }

        public BaseAsyncObserver(IAsyncObserver<TResult> observer)
        {
            _downstream = observer;
        }

        public virtual ValueTask OnSubscibeAsync(ICancelable disposable)
        {
            _upstream = disposable;
            return _downstream.OnSubscibeAsync(this);
        }

        public abstract ValueTask OnNextAsync(TSource value);

        public virtual ValueTask OnErrorAsync(Exception error)
        {
            if (IsDisposed)
                return new ValueTask();

            return _downstream.OnErrorAsync(error);
        }

        public virtual ValueTask OnCompletedAsync()
        {
            if (IsDisposed)
                return new ValueTask();

            return _downstream.OnCompletedAsync();
        }

        public virtual ValueTask OnFinallyAsync()
        {
            return _downstream.OnFinallyAsync();
        }

        public virtual void Dispose()
        {
            IsDisposed = true;
            _upstream.Dispose();
        }
    }

    class BaseAsyncObserver<T> : BaseAsyncObserver<T, T>
    {
        public BaseAsyncObserver(IAsyncObserver<T> observer) : base(observer)
        {
        }

        public override ValueTask OnNextAsync(T value)
        {
            if (IsDisposed)
                return new ValueTask();

            return _downstream.OnNextAsync(value);
        }
    }
}
