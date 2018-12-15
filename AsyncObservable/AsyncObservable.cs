using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public static class AsyncObservable
    {
        public static IAsyncObservable<TResult> Aggregate<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TResult> seed, Func<TResult, TSource, TResult> aggregator)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (seed == null)
                throw new ArgumentNullException(nameof(seed));
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));

            return new Aggregate<TSource, TResult>(source, seed, aggregator);
        }

        public static ValueTask<bool> AllAsync<T>(this IAsyncObservable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new AllAsyncObserver<T>(predicate);
            return source.ToTask(observer);
        }

        public static ValueTask<bool> AnyAsync<T>(this IAsyncObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new AnyAsyncObserver<T>();
            return source.ToTask(observer);
        }

        public static ValueTask<bool> AnyAsync<T>(this IAsyncObservable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new AnyPredicateAsyncObserver<T>(predicate);
            return source.ToTask(observer);
        }

        public static IAsyncObservable<T> Concat<T>(this IAsyncObservable<IAsyncObservable<T>> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new Concat<T>(source);
        }

        public static IAsyncObservable<T> Concat<T>(params IAsyncObservable<T>[] sources)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            return new ConcatEnumerable<T>(sources);
        }

        public static IAsyncObservable<T> Concat<T>(this IEnumerable<IAsyncObservable<T>> sources)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            return new ConcatEnumerable<T>(sources);
        }

        public static IAsyncObservable<T> Distinct<T>(this IAsyncObservable<T> sources)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            return new DistinctBy<T, T>(sources, v => v);
        }

        public static IAsyncObservable<T> DistinctBy<T, TSelect>(this IAsyncObservable<T> sources, Func<T, TSelect> selector)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new DistinctBy<T, TSelect>(sources, selector);
        }

        public static IAsyncObservable<T> DistinctUntilChanged<T>(this IAsyncObservable<T> sources)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            return new DistinctUntilChanged<T, T>(sources, v => v, EqualityComparer<T>.Default);
        }

        public static IAsyncObservable<T> DistinctUntilChanged<T, TSelect>(this IAsyncObservable<T> sources, Func<T, TSelect> selector)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new DistinctUntilChanged<T, TSelect>(sources, selector, EqualityComparer<TSelect>.Default);
        }

        public static IAsyncObservable<T> Do<T>(this IAsyncObservable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return new Do<T>(source, action);
        }

        public static IAsyncObservable<T> DropOnBackpressure<T>(this IAsyncObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new DropOnBackpressure<T>(source);
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

        public static ValueTask<T> FirstAsync<T>(this IAsyncObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new FirstAsyncObserver<T>();
            return source.ToTask(observer);
        }

        public static ValueTask<T> LastAsync<T>(this IAsyncObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new LastAsyncObserver<T>();
            return source.ToTask(observer);
        }


        public static IAsyncObservable<double> Max(this IAsyncObservable<double> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new MaxDouble(source);
        }

        public static IAsyncObservable<int> Max(this IAsyncObservable<int> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new MaxInt32(source);
        }

        public static IAsyncObservable<T> Never<T>()
        {
            return new Never<T>();
        }

        /// <summary>
        /// Orders the source by means of the given element selector ascendingly.
        /// </summary>
        /// <typeparam name="T">The element type of the source and result observables.</typeparam>
        /// <typeparam name="TSelect">The type of the selected sorting criterion.</typeparam>
        /// <param name="source">The observable to sort.</param>
        /// <param name="selector">The selector to select the sorting criterion.</param>
        /// <returns>The new observable instance.</returns>
        public static IOrderedAsyncObservable<T> OrderBy<T, TSelect>(this IAsyncObservable<T> source, Func<T, TSelect> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new OrderedAsyncObservable<T>(source, new AsscendingComparer<T, TSelect>(selector));
        }

        /// <summary>
        /// Orders the source by means of the given element selector and comparer ascendingly.
        /// </summary>
        /// <typeparam name="T">The element type of the source and result observables.</typeparam>
        /// <typeparam name="TSelect">The type of the selected sorting criterion.</typeparam>
        /// <param name="source">The observable to sort.</param>
        /// <param name="selector">The selector to select the sorting criterion.</param>
        /// <param name="comparer">The comparer to compare the sorting criterion.</param>
        /// <returns>The new observable instance.</returns>
        public static IOrderedAsyncObservable<T> OrderBy<T, TSelect>(this IAsyncObservable<T> source, Func<T, TSelect> selector, IComparer<TSelect> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new OrderedAsyncObservable<T>(source, new AsscendingComparer<T, TSelect>(selector, comparer));
        }

        /// <summary>
        /// Orders the source by means of the given element selector descendingly.
        /// </summary>
        /// <typeparam name="T">The element type of the source and result observables.</typeparam>
        /// <typeparam name="TSelect">The type of the selected sorting criterion.</typeparam>
        /// <param name="source">The observable to sort.</param>
        /// <param name="selector">The selector to select the sorting criterion.</param>
        /// <returns>The new observable instance.</returns>
        public static IOrderedAsyncObservable<T> OrderByDescending<T, TSelect>(this IAsyncObservable<T> source, Func<T, TSelect> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new OrderedAsyncObservable<T>(source, new DescendingComparer<T, TSelect>(selector));
        }

        /// <summary>
        /// Orders the source by means of the given element selector and comparer descendingly.
        /// </summary>
        /// <typeparam name="T">The element type of the source and result observables.</typeparam>
        /// <typeparam name="TSelect">The type of the selected sorting criterion.</typeparam>
        /// <param name="source">The observable to sort.</param>
        /// <param name="selector">The selector to select the sorting criterion.</param>
        /// <param name="comparer">The comparer to compare the sorting criterion.</param>
        /// <returns>The new observable instance.</returns>
        public static IOrderedAsyncObservable<T> OrderByDescending<T, TSelect>(this IAsyncObservable<T> source, Func<T, TSelect> selector, IComparer<TSelect> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new OrderedAsyncObservable<T>(source, new DescendingComparer<T, TSelect>(selector, comparer));
        }

        /// <summary>
        /// Orders the ordered observable by a further sorting criterion ascendingly.
        /// </summary>
        /// <typeparam name="T">The element type of the source and result observables.</typeparam>
        /// <typeparam name="TSelect">The type of the selected sorting criterion.</typeparam>
        /// <param name="source">The observable to sort.</param>
        /// <param name="selector">The selector to select the sorting criterion.</param>
        /// <returns>The new observable instance.</returns>
        public static IOrderedAsyncObservable<T> ThenBy<T, TSelect>(this IOrderedAsyncObservable<T> source, Func<T, TSelect> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return source.CreateOrderedObservable(selector, null, false);
        }

        /// <summary>
        /// Orders the ordered observable by a further sorting criterion ascendingly.
        /// </summary>
        /// <typeparam name="T">The element type of the source and result observables.</typeparam>
        /// <typeparam name="TSelect">The type of the selected sorting criterion.</typeparam>
        /// <param name="source">The observable to sort.</param>
        /// <param name="selector">The selector to select the sorting criterion.</param>
        /// <param name="comparer">The comparer to compare the sorting criterion.</param>
        /// <returns>The new observable instance.</returns>
        public static IOrderedAsyncObservable<T> ThenBy<T, TSelect>(this IOrderedAsyncObservable<T> source, Func<T, TSelect> selector, IComparer<TSelect> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return source.CreateOrderedObservable(selector, comparer, false);
        }

        /// <summary>
        /// Orders the ordered observable by a further sorting criterion descendingly.
        /// </summary>
        /// <typeparam name="T">The element type of the source and result observables.</typeparam>
        /// <typeparam name="TSelect">The type of the selected sorting criterion.</typeparam>
        /// <param name="source">The observable to sort.</param>
        /// <param name="selector">The selector to select the sorting criterion.</param>
        /// <returns>The new observable instance.</returns>
        public static IOrderedAsyncObservable<T> ThenByDescending<T, TSelect>(this IOrderedAsyncObservable<T> source, Func<T, TSelect> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return source.CreateOrderedObservable(selector, null, true);
        }

        /// <summary>
        /// Orders the ordered observable by a further sorting criterion descendingly.
        /// </summary>
        /// <typeparam name="T">The element type of the source and result observables.</typeparam>
        /// <typeparam name="TSelect">The type of the selected sorting criterion.</typeparam>
        /// <param name="source">The observable to sort.</param>
        /// <param name="selector">The selector to select the sorting criterion.</param>
        /// <param name="comparer">The comparer to compare the sorting criterion.</param>
        /// <returns>The new observable instance.</returns>
        public static IOrderedAsyncObservable<T> ThenByDescending<T, TSelect>(this IOrderedAsyncObservable<T> source, Func<T, TSelect> selector, IComparer<TSelect> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return source.CreateOrderedObservable(selector, comparer, true);
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

        public static IAsyncObservable<T> SampleFirst<T>(this IAsyncObservable<T> source, TimeSpan time, IAsyncScheduler scheduler)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (time < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(time));

            return new SampleFirst<T>(source, time, scheduler);
        }


        public static IAsyncObservable<TResult> Select<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new Select<TSource, TResult>.Sync(source, selector);
        }

        public static IAsyncObservable<TResult> Select<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TSource, ValueTask<TResult>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new Select<TSource, TResult>.Async(source, selector);
        }

        public static IAsyncObservable<TResult> Select<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TSource, CancellationToken, ValueTask<TResult>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new Select<TSource, TResult>.AsyncWithCancellation(source, selector);
        }

        public static IAsyncObservable<TResult> SelectMany<TSource, TResult>(this IAsyncObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new SelectMany<TSource, TResult>(source, selector);
        }

        public static IAsyncObservable<T> Skip<T>(this IAsyncObservable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return new Skip<T>(source, count);
        }

        public static IAsyncObservable<double> Sum(this IAsyncObservable<double> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new SumDouble(source);
        }

        public static IAsyncObservable<int> Sum(this IAsyncObservable<int> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new SumInt32(source);
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

            return new Where<T>.Sync(source, predicate);
        }

        public static IAsyncObservable<T> Where<T>(this IAsyncObservable<T> source, Func<T, ValueTask<bool>> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return new Where<T>.Async(source, predicate);
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

        public static IAsyncObservable<T> ToAsyncObservable<T>(this ValueTask<IEnumerable<T>> source)
            => new FromValueTaskEnumerable<T, IEnumerable<T>>(source);

        public static IAsyncObservable<T> ToAsyncObservable<T>(this ValueTask<List<T>> source)
            => new FromValueTaskEnumerable<T, List<T>>(source);

        public static IAsyncObservable<T> ToAsyncObservable<T>(this ValueTask<T[]> source)
            => new FromValueTaskEnumerable<T, T[]>(source);

        public static ValueTask<List<T>> ToListAsync<T>(this IAsyncObservable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new ToListAsyncObserver<T>();
            return source.ToTask(observer);
        }

        public static ValueTask SubscribeAsync<T>(this IAsyncObservable<T> source, Action<T> onNext = null, Action<Exception> onError = null, Action onCompleted = null, Action onFinally = null, CancellationToken ca = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new AnonymousAsyncObserver<T>.Sync(onNext, onError, onCompleted, onFinally, ca);
            return source.SubscribeAsync(observer);
        }

        public static ValueTask SubscribeAsync<T>(this IAsyncObservable<T> source, Func<T, ValueTask> onNext, Func<Exception, ValueTask> onError = null, Func<ValueTask> onCompleted = null, Func<ValueTask> onFinally = null, CancellationToken ca = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new AnonymousAsyncObserver<T>.Async(onNext, onError, onCompleted, onFinally, ca);
            return source.SubscribeAsync(observer);
        }

        public static IDisposable Subscribe<T>(this IAsyncObservable<T> source, Action<T> onNext = null, Action<Exception> onError = null, Action onCompleted = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var observer = new AnonymousAsyncObserver<T>.Sync(onNext, onError, onCompleted, null, default);
            source.SubscribeAsync(observer);
            return observer;
        }
    }
}
