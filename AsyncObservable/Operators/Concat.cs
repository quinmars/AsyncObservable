﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Concat<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<IAsyncObservable<T>> _source;

        public Concat(IAsyncObservable<IAsyncObservable<T>> source)
        {
            _source = source;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new OuterObserver(observer);
            return _source.SubscribeAsync(o);
        }

        class OuterObserver : ForwardingAsyncObserver<IAsyncObservable<T>, T>
        {
            InnerObserver _inner;

            public OuterObserver(IAsyncObserver<T> observer) : base(observer)
            {
            }

            public async override ValueTask OnNextAsync(IAsyncObservable<T> value)
            {
                if (IsCanceled)
                    return;

                var inner = new InnerObserver(this);
                try
                {
                    _inner = inner;
                    await value.SubscribeAsync(inner).ConfigureAwait(false);
                }
                catch (Exception error)
                {
                    _inner = null;
                    await SignalErrorAsync(error).ConfigureAwait(false);
                }
                _inner = null;
            }

            public ValueTask NextAsync(T value)
            {
                if (IsCanceled)
                    return default;

                return ForwardNextAsync(value);
            }

            public ValueTask ErrorAsync(Exception error)
            {
                if (IsCanceled)
                    return default;

                return SignalErrorAsync(error);
            }

            public override void Dispose()
            {
                Interlocked.Exchange(ref _inner, null)?.Dispose();
                base.Dispose();
            }
        }

        class InnerObserver : AsyncObserverBase, IAsyncObserver<T>
        {
            readonly OuterObserver _outer;

            public InnerObserver(OuterObserver outer)
            {
                _outer = outer;
            }

            public ValueTask OnSubscribeAsync(IDisposable cancelable)
            {
                SetUpstream(cancelable);
                return default;
            }

            public ValueTask OnNextAsync(T value)
            {
                if (IsCanceled)
                    return default;

                return _outer.NextAsync(value);
            }

            public ValueTask OnErrorAsync(Exception error)
            {
                if (IsCanceled)
                    return default;

                return _outer.ErrorAsync(error);
            }

            public ValueTask OnCompletedAsync() => default;
            public ValueTask OnFinallyAsync() => default;
        }
    }
}
