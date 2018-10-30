using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Zip<T1, T2> : IAsyncObservable<(T1, T2)>
    {
        readonly IAsyncObservable<T1> _source1;
        readonly IAsyncObservable<T2> _source2;

        public Zip(IAsyncObservable<T1> source1, IAsyncObservable<T2> source2)
        {
            _source1 = source1;
            _source2 = source2;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<(T1,T2)> observer)
        {
            var sink = new SharedSink(observer);

            var o1 = new Observer<T1>(sink);
            var o2 = new Observer<T2>(sink);

            sink._upstream1 = o1;
            sink._upstream2 = o2;

            var t1 = _source1.SubscribeAsync(o1);
            var t2 = _source2.SubscribeAsync(o2);

            await Task.WhenAll(t1.AsTask(), t2.AsTask());
        }

        class SharedSink : IAsyncCancelable
        {
            readonly IAsyncObserver<(T1, T2)> _downstream;

            public Observer<T1> _upstream1;
            public Observer<T2> _upstream2;

            public int _lock;
            public TaskCompletionSource<bool> _tcs;

            public int _lockDisposable;
            public TaskCompletionSource<bool> _tcsDisposable;

            public SharedSink(IAsyncObserver<(T1,T2)> observer)
            {
                _downstream = observer;
                _tcs = new TaskCompletionSource<bool>();
                _tcsDisposable = new TaskCompletionSource<bool>();
            }

            public ValueTask ForwardAsync()
            {
                if (Interlocked.Increment(ref _lock) == 2)
                {
                    return ForwardCoreAsync();
                }

                return new ValueTask(_tcs.Task);
            }

            public async ValueTask ForwardCoreAsync()
            {
                if (_upstream1._done || _upstream2._done)
                {
                    Dispose();
                    await _downstream.OnCompletedAsync();
                    return;
                }
                else if (_upstream1._exception != null || _upstream2._exception != null)
                {
                    return;
                }

                var val = (_upstream1._value, _upstream2._value);
                (_upstream1._value, _upstream2._value) = (default, default);
                _lock = 0;

                var tcs = _tcs;
                _tcs = new TaskCompletionSource<bool>();

                await _downstream.OnNextAsync(val);

                tcs.SetResult(true);
            }

            public bool IsDisposing => throw new NotImplementedException();
            public bool IsDisposed => throw new NotImplementedException();

            public void Dispose()
            {
                _upstream1._dispose.Dispose();
                _upstream2._dispose.Dispose();
            }

            public ValueTask DisposeAsync()
            {
                var d1 = _upstream1._dispose.DisposeAsync();
                var d2 = _upstream2._dispose.DisposeAsync();

                var t = Task.WhenAll(d1.AsTask(), d2.AsTask());
                return new ValueTask(t);
            }

            internal ValueTask ForwardSubscribeAsync()
            {
                if (Interlocked.Increment(ref _lockDisposable) == 2)
                {
                    return ForwardSubscribeCoreAsync();
                }
                return new ValueTask(_tcsDisposable.Task);
            }

            private async ValueTask ForwardSubscribeCoreAsync()
            {
                await _downstream.OnSubscibeAsync(this);
                _tcsDisposable.SetResult(true);
            }
        }

        private class Observer<T> : IAsyncObserver<T>
        {
            readonly SharedSink _sink;

            public T _value;
            public bool _done;
            public Exception _exception;
            public IAsyncCancelable _dispose;

            public Observer(SharedSink sink)
            {
                _sink = sink;
            }

            public ValueTask OnSubscibeAsync(IAsyncCancelable disposable)
            {
                _dispose = disposable;
                return _sink.ForwardSubscribeAsync();
            }

            public ValueTask OnNextAsync(T value)
            {
                _value = value;
                return _sink.ForwardAsync();
            }

            public ValueTask OnErrorAsync(Exception ex)
            {
                _exception = ex;
                return _sink.ForwardAsync();
            }

            public ValueTask OnCompletedAsync()
            {
                _done = true;
                return _sink.ForwardAsync();
            }
        }
    }
}
