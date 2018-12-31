using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Where<T>
    {
        public class Sync : IAsyncObservable<T>
        {
            readonly IAsyncObservable<T> _source;
            readonly Func<T, bool> _predicate;

            public Sync(IAsyncObservable<T> source, Func<T, bool> predicate)
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
                readonly Func<T, bool> _predicate;

                public Observer(IAsyncObserver<T> observer, Func<T, bool> predicate)
                    : base(observer)
                {
                    _predicate = predicate;
                }

                public override ValueTask OnNextAsync(T value)
                {
                    if (IsCanceled)
                        return default;

                    try
                    {
                        if (!_predicate(value))
                            return default;
                    }
                    catch (Exception ex)
                    {
                        return SignalErrorAsync(ex);
                    }

                    return ForwardNextAsync(value);
                }
            }
        }

        public class Async : IAsyncObservable<T>
        {
            readonly IAsyncObservable<T> _source;
            readonly Func<T, ValueTask<bool>> _predicate;

            public Async(IAsyncObservable<T> source, Func<T, ValueTask<bool>> predicate)
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
                        if (!await _predicate(value).ConfigureAwait(false))
                            return;

                        await ForwardNextAsync(value).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await SignalErrorAsync(ex).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
