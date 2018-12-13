using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class DistinctBy<T, TSelect> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly Func<T, TSelect> _selector;

        public DistinctBy(IAsyncObservable<T> source, Func<T, TSelect> selector)
        {
            _source = source;
            _selector = selector;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer, _selector);
            return _source.SubscribeAsync(o);
        }

        class Observer : ForwardingAsyncObserver<T>
        {
            readonly Func<T, TSelect> _selector;
            readonly HashSet<TSelect> _set;

            public Observer(IAsyncObserver<T> observer, Func<T, TSelect> selector)
                : base(observer)
            {
                _selector = selector;
                _set = new HashSet<TSelect>();
            }

            public override ValueTask OnNextAsync(T value)
            {
                if (IsCanceled)
                    return default;

                TSelect sval;
                try
                {
                    sval = _selector(value);
                }
                catch (Exception ex)
                {
                    return SignalErrorAsync(ex);
                }

                if (!_set.Add(sval))
                    return default;

                return ForwardNextAsync(value);
            }
        }
    }
}
