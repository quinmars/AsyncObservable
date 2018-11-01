﻿using System;
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

        private class Observer : BaseAsyncObserver<T>
        {
            readonly Action _action;

            public Observer(IAsyncObserver<T> observer, Action action)
                : base(observer)
            {
                _action = action;
            }

            public override ValueTask DisposeAsync()
            {
                _action();
                return _downstream.DisposeAsync();
            }
        }
    }
}