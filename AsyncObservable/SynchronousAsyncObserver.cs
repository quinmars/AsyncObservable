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
                return OnErrorAsync(ex);
            }
            return new ValueTask();
        }

        public async ValueTask OnCompletedAsync()
        {
            await DisposeAsync();
            _onCompleted();
        }

        public async ValueTask OnErrorAsync(Exception error)
        {
            await DisposeAsync();
            _onError(error);
        }

        public bool IsDisposing => _lock != 0;

        public ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _lock, 1) == 0)
            {
                return _downstream.DisposeAsync();
            }
            return new ValueTask();
        }
    }
}
