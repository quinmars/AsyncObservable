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

        public ValueTask OnSubscribeAsync(ICancelable disposable)
        {
            _upstream = disposable;
            return default;
        }

        public ValueTask OnNextAsync(T value)
        {
            if (IsDisposed)
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
            if (!IsDisposed)
                _onCompleted();

            return default;
        }

        public ValueTask OnErrorAsync(Exception error)
        {
            if (!IsDisposed)
                _onError(error);

            return default;
        }

        public bool IsDisposed { get; private set; }

        public ValueTask OnFinallyAsync()
        {
            return default;
        }

        public void Dispose()
        {
            IsDisposed = true;
            _upstream.Dispose();
        }
    }
}
