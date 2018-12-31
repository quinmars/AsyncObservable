using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class SelectMany<TSource, TResult> : IAsyncObservable<TResult>
    {
        readonly IAsyncObservable<TSource> _source;
        readonly Func<TSource, IEnumerable<TResult>> _selector;

        public SelectMany(IAsyncObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            _source = source;
            _selector = selector;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<TResult> observer)
        {
            var o = new Observer(observer, _selector);
            return _source.SubscribeAsync(o);
        }

        class Observer : ForwardingAsyncObserver<TSource, TResult>
        {
            readonly Func<TSource, IEnumerable<TResult>> _selector;

            public Observer(IAsyncObserver<TResult> observer, Func<TSource, IEnumerable<TResult>> selector)
                : base(observer)
            {
                _selector = selector;
            }

            public override ValueTask OnNextAsync(TSource value)
            {
                if (IsCanceled)
                    return default;

                IEnumerable<TResult> enumerable;
                try
                {
                    enumerable = _selector(value);
                }
                catch (Exception ex)
                {
                    return SignalErrorAsync(ex);
                }

                return Forward(enumerable);
            }

            private async ValueTask Forward(IEnumerable<TResult> enumerable)
            {
                try
                {
                    foreach (var item in enumerable)
                    {
                        if (IsCanceled)
                            return;

                        await ForwardNextAsync(item).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    await SignalErrorAsync(ex).ConfigureAwait(false);
                }
            }
        }
    }
}
