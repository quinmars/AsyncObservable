﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Take<T> : IAsyncObservable<T>
    {
        readonly IAsyncObservable<T> _source;
        readonly int _count;

        public Take(IAsyncObservable<T> source, int count)
        {
            _source = source;
            _count = count;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var o = new Observer(observer, _count);
            return _source.SubscribeAsync(o);
        }

        private class Observer : BaseAsyncObserver<T>
        {
            int _remaining;

            public Observer(IAsyncObserver<T> observer, int count)
                : base(observer)
            {
                _remaining = count;
            }

            public override ValueTask OnSubscibeAsync(ICancelable disposable)
            {
                if (_remaining == 0)
                    return ForwardFinalOnSubscribe(disposable);

                return base.OnSubscibeAsync(disposable);
            }

            private async ValueTask ForwardFinalOnSubscribe(ICancelable disposable)
            {
                _upstream = disposable;
                Dispose();
                await _downstream.OnSubscibeAsync(disposable);
                await _downstream.OnCompletedAsync();
            }

            public override ValueTask OnNextAsync(T value)
            {
                if (IsDisposed)
                    return new ValueTask();
                else if (_remaining == 1)
                    return ForwardLast(value);
                else if (_remaining > 1)
                {
                    _remaining--;
                    return _downstream.OnNextAsync(value);
                }

                return new ValueTask();
            }

            private async ValueTask ForwardLast(T value)
            {
                try
                {
                    _remaining--;
                    await _downstream.OnNextAsync(value);
                }
                catch (Exception ex)
                {
                    Dispose();
                    await _downstream.OnErrorAsync(ex);
                    return;
                }

                Dispose();
                await _downstream.OnCompletedAsync();
            }
        }
    }
}