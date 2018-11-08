using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Where<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly Func<T, bool> _predicate;

        public Where(IAsyncObservable<T> source, Func<T, bool> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer, _predicate);
            return _source.SubscribeAsync(o);
        }

        class Observer : BaseAsyncObserver<T>
        {
            readonly Func<T, bool> _predicate;

            public Observer(IAsyncObserver<T> observer, Func<T, bool> predicate)
                : base(observer)
            {
                _predicate = predicate;
            }

            public override ValueTask OnNextAsync(T value)
            {
                if (IsDisposed)
                    return default;

                try
                {
                    if (!_predicate(value))
                        return default;
                }
                catch (Exception ex)
                {
                    Dispose();
                    return _downstream.OnErrorAsync(ex);
                }

                return _downstream.OnNextAsync(value);
            }
        }
    }
}
