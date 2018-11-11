using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public static class AsyncObservable
    {
        public static IAsyncObservable<T> Concat<T>(this IAsyncObservable<IAsyncObservable<T>> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new Concat<T>(source);
        }

        public static IAsyncObservable<T> Do<T>(this IAsyncObservable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new Do<T>(source, action);
        }

        public static IAsyncObservable<T> Empty<T>()
        {
            return new Empty<T>();
        }

        public static IAsyncObservable<T> Finally<T>(this IAsyncObservable<T> source, Action action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new Finally<T>(source, action);
        }

        public static IAsyncObservable<T> Never<T>()
        {
            return new Never<T>();
        }

        public static IAsyncObservable<int> Range(int start, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new Range(start, count);
        }

        public static IAsyncObservable<T> Return<T>(T value)
        {
            return new Return<T>(value);
        }

        public static IAsyncObservable<TResult> Select<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new Select<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<T> Skip<T>(this IAsyncObservable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new Skip<T>(source, count);
        }

        public static IAsyncObservable<T> Take<T>(this IAsyncObservable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new Take<T>(source, count);
        }

        public static IAsyncObservable<T> Throw<T>(Exception ex)
        {
            return new Throw<T>(ex);
        }

        public static IAsyncObservable<TResult> Using<TResource, TResult>(Func<TResource> resourceFactory, Func<TResource, IAsyncObservable<TResult>> observableFactory)
            where TResource : IDisposable
        {
            if (resourceFactory == null)
                throw new ArgumentNullException(nameof(resourceFactory));
            if (observableFactory == null)
                throw new ArgumentNullException(nameof(observableFactory));

            return new Using<TResource, TResult>(resourceFactory, observableFactory);
        }

        public static IAsyncObservable<T> Where<T>(this IAsyncObservable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return new Where<T>(source, predicate);
        }


        public static IAsyncObservable<(T1, T2)> Zip<T1, T2>(this IAsyncObservable<T1> source1, IAsyncObservable<T2> source2)
        {
            if (source1 == null)
                throw new ArgumentNullException(nameof(source1));
            if (source2 == null)
                throw new ArgumentNullException(nameof(source2));

            return new Zip<T1, T2>(source1, source2);
        }

        public static IAsyncObservable<T> ToAsyncObservable<T>(this IObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new FromObservable<T>(source);
        }

        public static IAsyncObservable<T> ToAsyncObservable<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new FromEnumerable<T>(source);
        }

        public static ValueTask SubscribeAsync<T>(this IAsyncObservable<T> source, Action<T> onNext = null, Action<Exception> onError = null, Action onCompleted = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new SyncAsyncObserver<T>(onNext, onError, onCompleted);
            return source.SubscribeAsync(observer);
        }

        public static IDisposable Subscribe<T>(this IAsyncObservable<T> source, Action<T> onNext = null, Action<Exception> onError = null, Action onCompleted = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new SyncAsyncObserver<T>(onNext, onError, onCompleted);
            source.SubscribeAsync(observer);
            return observer;
        }
    }
}
