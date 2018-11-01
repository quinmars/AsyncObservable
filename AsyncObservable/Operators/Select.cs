using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Select<TSource, TResult> : IAsyncObservable<TResult>
    {
        readonly IAsyncObservable<TSource> _source;
        readonly Func<TSource, TResult> _selector;

        public Select(IAsyncObservable<TSource> source, Func<TSource, TResult> selector)
        {
            _source = source;
            _selector = selector;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<TResult> observer)
        {
            var o = new Observer(observer, _selector);
            return _source.SubscribeAsync(o);
        }

        private class Observer : BaseAsyncObserver<TSource, TResult>
        {
            readonly Func<TSource, TResult> _selector;

            public Observer(IAsyncObserver<TResult> observer, Func<TSource, TResult> selector)
                : base(observer)
            {
                _selector = selector;
            }

            public override ValueTask OnNextAsync(TSource value)
            {
                if (IsDisposed)
                    return new ValueTask();

                TResult v;
                try
                {
                    v = _selector(value);
                }
                catch (Exception ex)
                {
                    Dispose();
                    return _downstream.OnErrorAsync(ex);
                }

                return _downstream.OnNextAsync(v);
            }
        }
    }
}
