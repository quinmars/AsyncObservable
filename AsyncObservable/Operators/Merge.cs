using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Merge<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<IAsyncObservable<T>> _source;

        public Merge(IAsyncObservable<IAsyncObservable<T>> source)
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
            readonly CompositeDisposable _disposables;
            readonly AsyncLock _gate;

            readonly Dictionary<InnerObserver, Task> _innerObs;
            readonly object _lock;

            public OuterObserver(IAsyncObserver<T> observer) : base(observer)
            {
                _disposables = new CompositeDisposable();
                _gate = new AsyncLock();

                _innerObs = new Dictionary<InnerObserver, Task>();
                _lock = new object();
            }

            public override ValueTask OnNextAsync(IAsyncObservable<T> value)
            {
                if (!IsCanceled)
                {
                    var inner = new InnerObserver(this);
                    var vt = Subscribe(value, inner);

                    if (!vt.IsCompleted)
                    {
                        var t = vt.AsTask();
                        lock (_lock)
                        {
                            if (!t.IsCompleted)
                                _innerObs[inner] = t;
                        }
                    }
                }

                return default;
            }

            async ValueTask Subscribe(IAsyncObservable<T> obs, InnerObserver inner)
            {
                _disposables.Add(inner);
                try
                {
                    await obs.SubscribeAsync(inner).ConfigureAwait(false);
                }
                catch (Exception error)
                {
                    await SignalErrorAsync(error).ConfigureAwait(false);
                }
                finally
                {
                    _disposables.Remove(inner);
                    lock (_lock)
                        _innerObs.Remove(inner);
                }
            }

            public async ValueTask NextAsync(T value)
            {
                if (IsCanceled)
                    return;

                using (await _gate.LockAsync().ConfigureAwait(false))
                {
                    try
                    {
                        await ForwardNextAsync(value).ConfigureAwait(false);
                    }
                    catch (Exception error)
                    {
                        await SignalErrorAsync(error).ConfigureAwait(false);
                    }
                }
            }

            public async override ValueTask OnErrorAsync(Exception error)
            {
                if (IsCanceled)
                    return;

                await InnerCompletedAsync().ConfigureAwait(false);

                if (IsCanceled)
                    return;

                await ForwardErrorAsync(error).ConfigureAwait(false);
            }

            public async override ValueTask OnCompletedAsync()
            {
                if (IsCanceled)
                    return;

                await InnerCompletedAsync().ConfigureAwait(false);

                if (IsCanceled)
                    return;

                await ForwardCompletedAsync().ConfigureAwait(false);
            }

            public async override ValueTask OnFinallyAsync()
            {
                try
                {
                    await InnerCompletedAsync().ConfigureAwait(false);
                }
                finally
                {
                    await ForwardFinallyAsync().ConfigureAwait(false);
                }
            }

            public async ValueTask ErrorAsync(Exception error)
            {
                if (IsCanceled)
                    return;

                using (await _gate.LockAsync().ConfigureAwait(false))
                    await SignalErrorAsync(error).ConfigureAwait(false);
            }

            public override void Dispose()
            {
                _disposables.Dispose();
                base.Dispose();
            }

            private Task InnerCompletedAsync()
            {
                Task[] ts;
                lock (_lock)
                    ts = _innerObs.Values.ToArray();

                return Task.WhenAll(ts);
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
