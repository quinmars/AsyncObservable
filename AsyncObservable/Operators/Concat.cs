using System;
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

        class OuterObserver : BaseAsyncObserver<IAsyncObservable<T>, T>
        {
            InnerObserver _inner;

            public OuterObserver(IAsyncObserver<T> observer) : base(observer)
            {
            }

            public async override ValueTask OnNextAsync(IAsyncObservable<T> value)
            {
                if (IsDisposed)
                    return;

                var inner = new InnerObserver(this);
                try
                {
                    _inner = inner;
                    await value.SubscribeAsync(inner);
                }
                catch (Exception error)
                {
                    _inner = null;
                    Dispose();
                    await _downstream.OnErrorAsync(error);
                }
                _inner = null;
            }

            public ValueTask ForwardNextAsync(T value)
            {
                if (IsDisposed)
                    return default;

                return _downstream.OnNextAsync(value);
            }

            public ValueTask ForwardErrorAsync(Exception error)
            {
                if (IsDisposed)
                    return default;

                base.Dispose();
                return _downstream.OnErrorAsync(error);
            }

            public override void Dispose()
            {
                Interlocked.Exchange(ref _inner, null)?.Dispose();
                base.Dispose();
            }
        }

        class InnerObserver : IAsyncObserver<T>, ICancelable
        {
            readonly OuterObserver _outer;
            IDisposable _upstream;

            public bool IsDisposed { get; set; }

            public InnerObserver(OuterObserver outer)
            {
                _outer = outer;
            }

            public ValueTask OnSubscibeAsync(ICancelable cancelable)
            {
                _upstream = cancelable;
                return default;
            }

            public ValueTask OnNextAsync(T value)
            {
                if (IsDisposed)
                    return default;

                return _outer.ForwardNextAsync(value);
            }

            public ValueTask OnErrorAsync(Exception error)
            {
                if (IsDisposed)
                    return default;

                return _outer.ForwardErrorAsync(error);
            }

            public ValueTask OnCompletedAsync() => default;
            public ValueTask OnFinallyAsync() => default;

            public void Dispose()
            {
                Interlocked.Exchange(ref _upstream, null)?.Dispose();
                IsDisposed = true;
            }

        }
    }
}
