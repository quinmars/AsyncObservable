using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public class SynchronousAsyncObserver<T> : IAsyncObserver<T>, IAsyncCancelable
    {
        static readonly Action<T> OnNextNop = v => { };
        static readonly Action<Exception> OnErrorNop = ex => throw ex;
        static readonly Action OnCompletedNop = () => { };

        readonly Action<T> _onNext;
        readonly Action<Exception> _onError;
        readonly Action _onCompleted;

        IAsyncCancelable _downstream;
        int _lock;

        public SynchronousAsyncObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            _onNext = onNext ?? OnNextNop;
            _onError = onError ?? OnErrorNop;
            _onCompleted = onCompleted ?? OnCompletedNop;
        }

        public ValueTask OnSubscibeAsync(IAsyncCancelable disposable)
        {
            _downstream = disposable;
            return new ValueTask();
        }

        public ValueTask OnNextAsync(T value)
        {
            try
            {
                _onNext(value);
            }
            catch (Exception ex)
            {
                Dispose();
                _onError(ex);
            }
            return new ValueTask();
        }

        public ValueTask OnCompletedAsync()
        {
            Dispose();
            _onCompleted();
            return new ValueTask();
        }

        public ValueTask OnErrorAsync(Exception error)
        {
            Dispose();
            _onError(error);
            return new ValueTask();
        }

        public bool IsDisposing => _lock != 0;
        public bool IsDisposed => _downstream != null && _downstream.IsDisposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _lock, 1) == 0)
            {
                _downstream.Dispose();
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return _downstream.DisposeAsync();
        }
    }
}
