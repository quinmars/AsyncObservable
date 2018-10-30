using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public abstract class BaseAsyncObserver<TSource, TResult> : IAsyncObserver<TSource>
    {
        protected readonly IAsyncObserver<TResult> _downstream;
        protected IAsyncCancelable _upstream;

        public BaseAsyncObserver(IAsyncObserver<TResult> observer)
        {
            _downstream = observer;
        }

        public virtual ValueTask OnSubscibeAsync(IAsyncCancelable disposable)
        {
            _upstream = disposable;
            return _downstream.OnSubscibeAsync(disposable);
        }

        public abstract ValueTask OnNextAsync(TSource value);

        public virtual ValueTask OnErrorAsync(Exception error)
        {
            return _downstream.OnErrorAsync(error);
        }

        public ValueTask OnCompletedAsync()
        {
            return _downstream.OnCompletedAsync();
        }
    }

    public class BaseAsyncObserver<T> : BaseAsyncObserver<T, T>
    {
        public BaseAsyncObserver(IAsyncObserver<T> observer) : base(observer)
        {
        }

        public override ValueTask OnNextAsync(T value)
        {
            return _downstream.OnNextAsync(value);
        }
    }
}
