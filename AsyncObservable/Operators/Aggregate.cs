﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Aggregate<TSource, TResult> : IAsyncObservable<TResult>
    {
        readonly IAsyncObservable<TSource> _source;
        readonly Func<TResult> _seed;
        readonly Func<TResult, TSource, TResult> _aggregator;

        public Aggregate(IAsyncObservable<TSource> source, Func<TResult> seed, Func<TResult, TSource, TResult> aggregator)
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

        class Observer : BaseAsyncObserver<TSource, TResult>
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
                if (IsDisposed)
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
                        Dispose();
                        return _downstream.OnErrorAsync(error);
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
                    Dispose();
                    return _downstream.OnErrorAsync(error);
                }

                return default;
            }

            public override async ValueTask OnCompletedAsync()
            {
                if (IsDisposed)
                    return;
                await _downstream.OnNextAsync(_accumulation);
                await _downstream.OnCompletedAsync();
            }
        }
    }
}