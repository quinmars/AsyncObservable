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

        class Observer : ForwardingAsyncObserver<T>
        {
            readonly Action _action;

            public Observer(IAsyncObserver<T> observer, Action action)
                : base(observer)
            {
                _action = action;
            }

            public override async ValueTask OnFinallyAsync()
            {
                try
                {
                    _action();
                }
                finally
                {
                    await ForwardFinallyAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
