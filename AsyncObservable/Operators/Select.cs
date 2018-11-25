using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    static class Select<TSource, TResult>
    {
        public class Sync : IAsyncObservable<TResult>
        {
            readonly IAsyncObservable<TSource> _source;
            readonly Func<TSource, TResult> _selector;

            public Sync(IAsyncObservable<TSource> source, Func<TSource, TResult> selector)
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
                readonly Func<TSource, TResult> _selector;

                public Observer(IAsyncObserver<TResult> observer, Func<TSource, TResult> selector)
                    : base(observer)
                {
                    _selector = selector;
                }

                public override ValueTask OnNextAsync(TSource value)
                {
                    if (IsCanceled)
                        return default;

                    TResult v;
                    try
                    {
                        v = _selector(value);
                    }
                    catch (Exception ex)
                    {
                        return SignalErrorAsync(ex);
                    }

                    return ForwardNextAsync(v);
                }
            }
        }

        public class Async : IAsyncObservable<TResult>
        {
            readonly IAsyncObservable<TSource> _source;
            readonly Func<TSource, ValueTask<TResult>> _selector;

            public Async(IAsyncObservable<TSource> source, Func<TSource, ValueTask<TResult>> selector)
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
                readonly Func<TSource, ValueTask<TResult>> _selector;

                public Observer(IAsyncObserver<TResult> observer, Func<TSource, ValueTask<TResult>> selector)
                    : base(observer)
                {
                    _selector = selector;
                }

                public override async ValueTask OnNextAsync(TSource value)
                {
                    if (IsCanceled)
                        return;

                    try
                    {
                        var v = await _selector(value);
                        await ForwardNextAsync(v);
                    }
                    catch (Exception ex)
                    {
                        await SignalErrorAsync(ex);
                    }

                }
            }
        }
    }
}
