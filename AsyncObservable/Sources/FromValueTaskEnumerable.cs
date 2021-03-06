﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class FromValueTaskEnumerable<T, TEnumerable> : IAsyncObservable<T>
        where TEnumerable : IEnumerable<T>
    {
        readonly ValueTask<TEnumerable> _enumerable;

        public FromValueTaskEnumerable(ValueTask<TEnumerable> enumerable)
        {
            _enumerable = enumerable;
        }

        public async ValueTask SubscribeAsync(IAsyncObserver<T> observer)
        {
            var disposable = new BooleanDisposable();

            try
            {
                await observer.OnSubscribeAsync(disposable).ConfigureAwait(false);
                var items = await _enumerable.ConfigureAwait(false);
                foreach (var item in items)
                {
                    if (disposable.IsDisposed)
                        break;

                    var t = observer.OnNextAsync(item);

                    try
                    {
                        await t.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await observer.OnErrorAsync(ex).ConfigureAwait(false);
                        break;
                    }
                }

                if (!disposable.IsDisposed)
                    await observer.OnCompletedAsync().ConfigureAwait(false);
            }
            finally
            {
                await observer.OnFinallyAsync().ConfigureAwait(false);
            }
        }
    }
}
