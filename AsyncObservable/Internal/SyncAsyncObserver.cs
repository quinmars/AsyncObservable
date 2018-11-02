using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class SyncAsyncObserver<T> : IAsyncObserver<T>, ICancelable
    {
        static readonly Action<T> OnNextNop = v => { };
        static readonly Action<Exception> OnErrorNop = ex => throw ex;
        static readonly Action OnCompletedNop = () => { };

        readonly Action<T> _onNext;
        readonly Action<Exception> _onError;
        readonly Action _onCompleted;

        ICancelable _upstream;

        public SyncAsyncObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            _onNext = onNext ?? OnNextNop;
            _onError = onError ?? OnErrorNop;
            _onCompleted = onCompleted ?? OnCompletedNop;
        }

        public ValueTask OnSubscibeAsync(ICancelable disposable)
        {
            _upstream = disposable;
            return new ValueTask();
        }

        public ValueTask OnNextAsync(T value)
        {
            if (IsDisposed)
                return new ValueTask();

            try
            {
                _onNext(value);
            }
            catch (Exception ex)
            {
                Dispose();
                return OnErrorAsync(ex);
            }
            return new ValueTask();
        }

        public ValueTask OnCompletedAsync()
        {
            if (!IsDisposed)
                _onCompleted();
            return new ValueTask();
        }

        public ValueTask OnErrorAsync(Exception error)
        {
            if (!IsDisposed)
                _onError(error);
            return new ValueTask();
        }

        public bool IsDisposed { get; private set; }

        public ValueTask OnFinallyAsync()
        {
            return new ValueTask();
        }

        public void Dispose()
        {
            IsDisposed = true;
            _upstream.Dispose();
        }
    }
}
