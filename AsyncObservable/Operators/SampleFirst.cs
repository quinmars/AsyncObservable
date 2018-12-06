using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class SampleFirst<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly TimeSpan _time;
        readonly IAsyncScheduler _scheduler;
 
        public SampleFirst(IAsyncObservable<T> source, TimeSpan time, IAsyncScheduler scheduler)
        {
            _source = source;
            _time = time;
            _scheduler = scheduler;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer, _time, _scheduler);
            return _source.SubscribeAsync(o);
        }

        class Observer : ForwardingAsyncObserver<T>
        {
            readonly TimeSpan _time;
            readonly IAsyncScheduler _scheduler;
            Task _current = Task.CompletedTask;
            CancellationTokenSource _cts;

            public Observer(IAsyncObserver<T> observer, TimeSpan time, IAsyncScheduler scheduler)
                : base(observer)
            {
                _cts = new CancellationTokenSource();
                _time = time;
                _scheduler = scheduler;
            }

            public override ValueTask OnNextAsync(T value)
            {
                if (IsCanceled || !_current.IsCompleted)
                    return default;

                _current = OnNextCore(value);
                return default;
            }

            private async Task OnNextCore(T value)
            {
                var t = ForwardNextAsync(value).AsTask();
                var delay = _scheduler.Delay(_time, _cts.Token);

                try
                {
                    await Task.WhenAll(t, delay);
                }
                catch (Exception error)
                {
                    if (!IsCanceled)
                        await SignalErrorAsync(error);
                }
            }

            public async override ValueTask OnErrorAsync(Exception error)
            {
                await _current;
                _current = Task.CompletedTask;
                await base.OnErrorAsync(error);
            }

            public async override ValueTask OnCompletedAsync()
            {
                await _current;
                _current = Task.CompletedTask;
                await base.OnCompletedAsync();
            }

            public async override ValueTask OnFinallyAsync()
            {
                try
                {
                    await _current;
                    _current = Task.CompletedTask;
                    _cts.Dispose();
                }
                finally
                {
                    await base.OnFinallyAsync();
                }
            }

            public override void Dispose()
            {
                base.Dispose();
                _cts.Cancel();
            }
        }
    }
}
