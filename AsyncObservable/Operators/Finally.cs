using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Finally<T> : IAsyncObservable<T>
    {
        readonly Action _action;
        readonly IAsyncObservable<T> _source;

        public Finally(IAsyncObservable<T> source, Action action)
        {
            _action = action;
            _source = source;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer, _action);
            return _source.SubscribeAsync(o);
        }

        private class Observer : BaseAsyncObserver<T>, IAsyncCancelable
        {
            readonly Action _action;
            int _lock;

            public Observer(IAsyncObserver<T> observer, Action action)
                : base(observer)
            {
                _action = action;
            }

            public override ValueTask OnSubscibeAsync(IAsyncCancelable disposable)
            {
                _upstream = disposable;
                return _downstream.OnSubscibeAsync(this);
            }

            public bool IsDisposing => _lock != 0;
            public bool IsDisposed => _upstream != null && _upstream.IsDisposed;

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _lock, 1) == 0)
                {
                    _upstream.Dispose();
                    _action();
                }
            }

            public ValueTask DisposeAsync()
            {
                Dispose();
                return _upstream.DisposeAsync();
            }
        }
    }
}
