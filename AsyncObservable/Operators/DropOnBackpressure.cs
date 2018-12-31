using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class DropOnBackpressure<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly int _count;

        public DropOnBackpressure(IAsyncObservable<T> source)
        {
            _source = source;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer);
            return _source.SubscribeAsync(o);
        }

        class Observer : ForwardingAsyncObserver<T>
        {
            Task _current = Task.CompletedTask;

            public Observer(IAsyncObserver<T> observer)
                : base(observer)
            {
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
                var t = ForwardNextAsync(value);
                try
                {
                    await t.ConfigureAwait(false);
                }
                catch (Exception error)
                {
                    if (!IsCanceled)
                        await SignalErrorAsync(error).ConfigureAwait(false);
                }
            }

            public async override ValueTask OnErrorAsync(Exception error)
            {
                await _current.ConfigureAwait(false);
                _current = Task.CompletedTask;
                await base.OnErrorAsync(error).ConfigureAwait(false);
            }

            public async override ValueTask OnCompletedAsync()
            {
                await _current.ConfigureAwait(false);
                _current = Task.CompletedTask;
                await base.OnCompletedAsync().ConfigureAwait(false);
            }

            public async override ValueTask OnFinallyAsync()
            {
                await _current.ConfigureAwait(false);
                _current = Task.CompletedTask;
                await base.OnFinallyAsync().ConfigureAwait(false);
            }

        }
    }
}
