using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class AsyncWhere<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly Func<T, ValueTask<bool>> _predicate;

        public AsyncWhere(IAsyncObservable<T> source, Func<T, ValueTask<bool>> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer, _predicate);
            return _source.SubscribeAsync(o);
        }

        class Observer : ForwardingAsyncObserver<T>
        {
            readonly Func<T, ValueTask<bool>> _predicate;

            public Observer(IAsyncObserver<T> observer, Func<T, ValueTask<bool>> predicate)
                : base(observer)
            {
                _predicate = predicate;
            }

            public override async ValueTask OnNextAsync(T value)
            {
                if (IsCanceled)
                    return;

                try
                {
                    if (! await _predicate(value))
                        return;

                    await ForwardNextAsync(value);
                }
                catch (Exception ex)
                {
                    await SignalErrorAsync(ex);
                }
            }
        }
    }
}
