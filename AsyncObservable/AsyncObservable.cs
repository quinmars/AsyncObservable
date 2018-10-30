using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public static class AsyncObservable
    {
        public static IAsyncObservable<T> Do<T>(this IAsyncObservable<T> source, Action<T> action)
        {
            return new Do<T>(source, action);
        }

        public static IAsyncObservable<T> Empty<T>()
        {
            return new Empty<T>();
        }

        public static IAsyncObservable<T> Finally<T>(this IAsyncObservable<T> source, Action action)
        {
            return new Finally<T>(source, action);
        }

        public static IAsyncObservable<T> Never<T>()
        {
            return new Never<T>();
        }

        public static IAsyncObservable<int> Range(int start, int count)
        {
            return new Range(start, count);
        }

        public static IAsyncObservable<T> Return<T>(T value)
        {
            return new Return<T>(value);
        }

        public static IAsyncObservable<TResult> Select<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TSource, TResult> selector)
        {
            return new Select<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<T> Take<T>(this IAsyncObservable<T> source, int count)
        {
            return new Take<T>(source, count);
        }

        public static IAsyncObservable<T> Throw<T>(Exception ex)
        {
            return new Throw<T>(ex);
        }

        public static IAsyncObservable<TResult> Using<TResource, TResult>(Func<TResource> resourceFactory, Func<TResource, IAsyncObservable<TResult>> observableFactory)
            where TResource : IDisposable
        {
            return new Using<TResource, TResult>(resourceFactory, observableFactory);
        }

        public static IAsyncObservable<(T1, T2)> Zip<T1, T2>(this IAsyncObservable<T1> source1, IAsyncObservable<T2> source2)
        {
            return new Zip<T1, T2>(source1, source2);
        }

        public static async ValueTask<IAsyncDisposable> SubscribeAsync<T>(this IAsyncObservable<T> source, Action<T> onNext = null, Action<Exception> onError = null, Action onCompleted = null)
        {
            var observer = new SynchronousAsyncObserver<T>(onNext, onError, onCompleted);
            await source.SubscribeAsync(observer);
            return observer;
        }
    }
}
