using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    static class AggregateImpl
    {
        static public IAsyncObservable<T> Create<T, TAggregator>(IAsyncObservable<T> source, TAggregator aggregator, bool intermediateResults)
            where TAggregator : struct, IAggreagator<T, T>
        {
            if (intermediateResults)
                return new Aggregate<T, T>.WithInterfaceIntermediateResults<TAggregator>(source, aggregator);
            else
                return new Aggregate<T,T>.WithInterface<TAggregator>(source, aggregator);
        }
    }

    static class Aggregate<TSource, TResult>
    {
        public class WithDelegate : IAsyncObservable<TResult>
        {
            readonly IAsyncObservable<TSource> _source;
            readonly Func<TResult> _seed;
            readonly Func<TResult, TSource, TResult> _aggregator;

            public WithDelegate(IAsyncObservable<TSource> source, Func<TResult> seed, Func<TResult, TSource, TResult> aggregator)
            {
                _source = source;
                _seed = seed;
                _aggregator = aggregator;
            }

            public ValueTask SubscribeAsync(IAsyncObserver<TResult> observer)
            {
                var o = new Observer(observer, _seed, _aggregator);
                return _source.SubscribeAsync(o);
            }

            class Observer : ForwardingAsyncObserver<TSource, TResult>
            {
                readonly Func<TResult> _seed;
                readonly Func<TResult, TSource, TResult> _aggregator;

                TResult _accumulation;
                bool _first;

                public Observer(IAsyncObserver<TResult> observer, Func<TResult> seed, Func<TResult, TSource, TResult> aggregator)
                    : base(observer)
                {
                    _seed = seed;
                    _aggregator = aggregator;
                    _first = true;
                }

                public override ValueTask OnNextAsync(TSource value)
                {
                    if (IsCanceled)
                        return default;

                    TResult acc;
                    if (_first)
                    {
                        _first = false;
                        try
                        {
                            acc = _seed();
                        }
                        catch (Exception error)
                        {
                            return SignalErrorAsync(error);
                        }
                    }
                    else
                        acc = _accumulation;

                    try
                    {
                        _accumulation = _aggregator(acc, value);
                    }
                    catch (Exception error)
                    {
                        return SignalErrorAsync(error);
                    }

                    return default;
                }

                public override async ValueTask OnCompletedAsync()
                {
                    if (IsCanceled)
                        return;
                    await ForwardNextAsync(_accumulation).ConfigureAwait(false);
                    await ForwardCompletedAsync().ConfigureAwait(false);
                }
            }
        }



        public class WithInterface<TAggregator> : IAsyncObservable<TResult>
            where TAggregator : struct, IAggreagator<TSource, TResult>
        {
            readonly IAsyncObservable<TSource> _source;
            readonly TAggregator _aggregator;

            public WithInterface(IAsyncObservable<TSource> source, TAggregator aggregator)
            {
                _source = source;
                _aggregator = aggregator;
            }

            public ValueTask SubscribeAsync(IAsyncObserver<TResult> observer)
            {
                var o = new Observer(observer, _aggregator);
                return _source.SubscribeAsync(o);
            }

            class Observer : ForwardingAsyncObserver<TSource, TResult>
            {
                TAggregator _aggregator; // Possibly mutable struct. Do not make this readonly.

                public Observer(IAsyncObserver<TResult> observer, TAggregator aggregator)
                    : base(observer)
                {
                    _aggregator = aggregator;
                    _aggregator.Init();
                }

                public override ValueTask OnNextAsync(TSource value)
                {
                    if (!IsCanceled)
                    {
                        _aggregator.Add(value);
                    }
                    return default;
                }

                public override async ValueTask OnCompletedAsync()
                {
                    if (IsCanceled)
                        return;

                    if (!_aggregator.HasValue)
                    {
                        await ForwardErrorAsync(new InvalidOperationException("Sequence contains no elements!")).ConfigureAwait(false);
                        return;
                    }

                    await ForwardNextAsync(_aggregator.Value).ConfigureAwait(false);
                    await ForwardCompletedAsync().ConfigureAwait(false);
                }
            }
        }

        public class WithInterfaceIntermediateResults<TAggregator> : IAsyncObservable<TResult>
            where TAggregator : struct, IAggreagator<TSource, TResult>
        {
            readonly IAsyncObservable<TSource> _source;
            readonly TAggregator _aggregator;

            public WithInterfaceIntermediateResults(IAsyncObservable<TSource> source, TAggregator aggregator)
            {
                _source = source;
                _aggregator = aggregator;
            }

            public ValueTask SubscribeAsync(IAsyncObserver<TResult> observer)
            {
                var o = new Observer(observer, _aggregator);
                return _source.SubscribeAsync(o);
            }

            class Observer : ForwardingAsyncObserver<TSource, TResult>
            {
                TAggregator _aggregator; // Possibly mutable struct. Do not make this readonly.

                public Observer(IAsyncObserver<TResult> observer, TAggregator aggregator)
                    : base(observer)
                {
                    _aggregator = aggregator;
                    _aggregator.Init();
                }

                public override ValueTask OnNextAsync(TSource value)
                {
                    if (!IsCanceled)
                    {
                        if (_aggregator.Add(value))
                            return ForwardNextAsync(_aggregator.Value);
                    }
                    return default;
                }

                public override ValueTask OnCompletedAsync()
                {
                    if (IsCanceled)
                        return default;

                    if (!_aggregator.HasValue)
                        return ForwardErrorAsync(new InvalidOperationException("Sequence contains no elements!"));

                    return ForwardCompletedAsync();
                }
            }
        }
    }
}
