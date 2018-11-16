using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Do<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly Action<T> _action;

        public Do(IAsyncObservable<T> source, Action<T> action)
        {
            _source = source;
            _action = action;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer, _action);
            return _source.SubscribeAsync(o);
        }

        class Observer : BaseAsyncObserver<T>
        {
            readonly Action<T> _action;

            public Observer(IAsyncObserver<T> observer, Action<T> action)
                : base(observer)
            {
                _action = action;
            }

            public override ValueTask OnNextAsync(T value)
            {
                if (IsDisposed)
                    return default;

                try
                {
                    _action(value);
                }
                catch (Exception ex)
                {
                    return SignalErrorAsync(ex);
                }
                return ForwardNextAsync(value);
            }
        }
    }
}
