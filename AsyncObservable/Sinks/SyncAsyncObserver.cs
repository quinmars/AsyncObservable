﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class SyncAsyncObserver<T> : AsyncObserverBase, IAsyncObserver<T>
    {
        static readonly Action<T> OnNextNop = v => { };
        static readonly Action<Exception> OnErrorNop = ex => throw ex;
        static readonly Action OnCompletedNop = () => { };

        readonly Action<T> _onNext;
        readonly Action<Exception> _onError;
        readonly Action _onCompleted;

        public SyncAsyncObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            _onNext = onNext ?? OnNextNop;
            _onError = onError ?? OnErrorNop;
            _onCompleted = onCompleted ?? OnCompletedNop;
        }

        public ValueTask OnSubscribeAsync(IDisposable disposable)
        {
            SetUpstream(disposable);
            return default;
        }

        public ValueTask OnNextAsync(T value)
        {
            if (IsCanceled)
                return default;

            try
            {
                _onNext(value);
            }
            catch (Exception ex)
            {
                Dispose();
                return OnErrorAsync(ex);
            }
            return default;
        }

        public ValueTask OnCompletedAsync()
        {
            if (!IsCanceled)
                _onCompleted();

            return default;
        }

        public ValueTask OnErrorAsync(Exception error)
        {
            if (!IsCanceled)
                _onError(error);

            return default;
        }

        public ValueTask OnFinallyAsync() => default;
    }
}