using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Skip<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly int _count;

        public Skip(IAsyncObservable<T> source, int count)
        {
            _source = source;
            _count = count;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer, _count);
            return _source.SubscribeAsync(o);
        }

        class Observer : BaseAsyncObserver<T>
        {
            int _remaining;

            public Observer(IAsyncObserver<T> observer, int count)
                : base(observer)
            {
                _remaining = count;
            }

            public override ValueTask OnNextAsync(T value)
            {
                if (IsCanceled)
                    return default;
                else if (_remaining == 0)
                    return ForwardNextAsync(value);
                else
                    _remaining--;

                return default;
            }
        }
    }
}
