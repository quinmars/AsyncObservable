using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
                        var v = await _selector(value).ConfigureAwait(false);
                        await ForwardNextAsync(v).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await SignalErrorAsync(ex).ConfigureAwait(false);
                    }

                }
            }
        }

        public class AsyncWithCancellation : IAsyncObservable<TResult>
        {
            readonly IAsyncObservable<TSource> _source;
            readonly Func<TSource, CancellationToken, ValueTask<TResult>> _selector;

            public AsyncWithCancellation(IAsyncObservable<TSource> source, Func<TSource, CancellationToken, ValueTask<TResult>> selector)
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
                readonly Func<TSource, CancellationToken, ValueTask<TResult>> _selector;
                CancellationTokenSource _caSource;

                public Observer(IAsyncObserver<TResult> observer, Func<TSource, CancellationToken, ValueTask<TResult>> selector)
                    : base(observer)
                {
                    _selector = selector;
                    _caSource = new CancellationTokenSource();
                }

                public override async ValueTask OnNextAsync(TSource value)
                {
                    if (IsCanceled)
                        return;

                    try
                    {
                        var v = await _selector(value, _caSource.Token).ConfigureAwait(false);
                        await ForwardNextAsync(v).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (!IsCanceled)
                            await SignalErrorAsync(ex).ConfigureAwait(false);
                    }

                }

                public override void Dispose()
                {
                    base.Dispose();

                    var cas = Interlocked.Exchange(ref _caSource, null);

                    if (cas != null)
                    {
                        cas.Cancel();
                        cas.Dispose();
                    }
                }

                public override ValueTask OnFinallyAsync()
                {
                    Interlocked.Exchange(ref _caSource, null)?.Dispose();
                    return base.OnFinallyAsync();
                }
            }
        }
    }
}
