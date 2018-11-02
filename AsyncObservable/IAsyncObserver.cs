﻿using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    public interface IAsyncObserver<in T>
    {
        ValueTask OnSubscibeAsync(ICancelable cancelable);
        ValueTask OnNextAsync(T value);
        ValueTask OnCompletedAsync();
        ValueTask OnErrorAsync(Exception error);
        ValueTask OnFinallyAsync();
    }
}
