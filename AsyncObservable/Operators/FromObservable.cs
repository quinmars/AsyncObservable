using async_enumerable_dotnet.impl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class FromObservable<T> : IAsyncObservable<T>
    {
        readonly IObservable<T> _source;

        public FromObservable(IObservable<T> source)
        {
            _source = source;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var obs = new Observer(observer);

            var d = _source.Subscribe(obs);
            obs.SetResource(d);

            return obs.Run();
        }

        /*
         * The Observer implementation is very inspired by David Karnok's
         * IObserver/IAsyncEnumerator adapter. And by his educational and
         * worth reading writing: 
         * https://github.com/akarnokd/async-enumerable-dotnet/wiki/Writing-operators
         */
        class Observer : IObserver<T>, ICancelable
        {
            readonly IAsyncObserver<T> _observer;

            readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
            Exception _error;
            bool _done;

            TaskCompletionSource<bool> _tcs;
            IDisposable _upstream;

            public Observer(IAsyncObserver<T> observer)
            {
                _observer = observer;
            }

            public bool IsDisposed { get; set; }

            public void Dispose()
            {
                Interlocked.Exchange(ref _upstream, null)?.Dispose();
                IsDisposed = true;
            }

            public async ValueTask Run()
            {
                await _observer.OnSubscribeAsync(this);

                var done = false;

                while (true)
                {
                    while (true)
                    {
                        if (IsDisposed)
                            goto Dispose;

                        done = Volatile.Read(ref _done);

                        if (_queue.TryDequeue(out var item))
                        {
                            var t = _observer.OnNextAsync(item);

                            try
                            {
                                await t;
                            }
                            catch (Exception ex)
                            {
                                await _observer.OnErrorAsync(ex);
                                goto Dispose;
                            }
                        }
                        else if (done)
                            goto Done;
                        else
                            break;
                    }

                    await ResumeHelper.Await(ref _tcs);
                    ResumeHelper.Clear(ref _tcs);
                } 

            Done:
                if (!IsDisposed)
                {
                    if (_error == null)
                        await _observer.OnCompletedAsync();
                    else
                        await _observer.OnErrorAsync(_error);
                }

            Dispose:
                Dispose();

                await _observer.OnFinallyAsync();
            }


            public void SetResource(IDisposable d)
            {
                _upstream = d;
            }

            /*
             * IObserver implementation
             */
        public void OnCompleted()
            {
                _done = true;
                Signal();
            }

            public void OnError(Exception error)
            {
                _error = error;
                _done = true;
                Signal();
            }

            public void OnNext(T value)
            {
                _queue.Enqueue(value);
                Signal();
            }

            private void Signal()
            {
                ResumeHelper.Resume(ref _tcs);
            }
        }
    }
}
