using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class DistinctUntilChanged<T, TSelect> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly Func<T, TSelect> _selector;
        readonly IEqualityComparer<TSelect> _comparer;

        public DistinctUntilChanged(IAsyncObservable<T> source, Func<T, TSelect> selector, IEqualityComparer<TSelect> comparer)
        {
            _source = source;
            _selector = selector;
            _comparer = comparer;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer, _selector, _comparer);
            return _source.SubscribeAsync(o);
        }

        class Observer : ForwardingAsyncObserver<T>
        {
            readonly Func<T, TSelect> _selector;
            readonly IEqualityComparer<TSelect> _comparer;
            TSelect lastValue;
            bool hasLastValue;

            public Observer(IAsyncObserver<T> observer, Func<T, TSelect> selector, IEqualityComparer<TSelect> comparer)
                : base(observer)
            {
                _selector = selector;
                _comparer = comparer;
            }

            public override ValueTask OnNextAsync(T value)
            {
                if (IsCanceled)
                    return default;

                TSelect sval;
                try
                {
                    sval = _selector(value);
                    if (hasLastValue && _comparer.Equals(sval, lastValue))
                        return default;
                }
                catch (Exception ex)
                {
                    return SignalErrorAsync(ex);
                }

                hasLastValue = true;
                lastValue = sval;

                return ForwardNextAsync(value);
            }
        }
    }
}
