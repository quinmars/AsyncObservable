using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    static class ToTaskAsyncObserver
    {
        public static async ValueTask<T> ToTask<T>(this IAsyncObservable<T> source, ToTaskAsyncObserver<T> observer)
        {
            await source.SubscribeAsync(observer);
            if (observer.Error != null)
                throw observer.Error;

            if (!observer.HasValue)
                throw new InvalidOperationException("Sequence has no elements");

            return observer.Value;
        }
    }

    abstract class ToTaskAsyncObserver<T> : AsyncObserverBase, IAsyncObserver<T>
    {
        //
        // Cancellation support should be added some day
        //
        public Exception Error { get; protected set; }
        public T Value { get; protected set; }
        public bool HasValue { get; protected set; }

        public ValueTask OnSubscribeAsync(IDisposable cancelable)
        {
            SetUpstream(cancelable);
            return default;
        }

        public abstract ValueTask OnNextAsync(T value);

        public ValueTask OnErrorAsync(Exception error)
        {
            Error = error;
            return default;
        }

        public ValueTask OnCompletedAsync() => default;

        public ValueTask OnFinallyAsync() => default;

    }
}
