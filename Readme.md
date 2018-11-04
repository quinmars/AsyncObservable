# AsyncObservable

Around one year ago Bart de Smert started to work on `IAsyncObservable` in public.
`IAsyncObservable` offers some advantages over the classical Rx interface,
especially when slow asynchronous consumers are involved. His implementation is
a one to one translation of the Rx.NET contract, so he unfortunately omitted the 
opportunity to correct some deficits of the original synchronous contract. Namely:

  - __Synchronous Cancellation__: The following will print the numbers from 0 to
  999 because there is no way for the `Take` operator to stop `Range`.
 ```csharp
Range(0,1000)
    .Do(Console.WriteLine($"{i}"))
    .Take(2)
    .Subscribe(); 
 ```
  - __IDisposable Contract__: A resource should not be disposed while it is
  still in use. In Rx one `IDisposable` (returned by `Subscribe`) is used for
  two conflicting purposes. On the one hand it is used as a cancellation trigger
  to stop the parent observable production, on the other hand it is used
  for resource cleanup. Cancellation demands to be fast, so that
  not to many neglected values are generated. Contrary, resource cleanup requests
  to be late so that a resource will not be disposed  while it is still in use, for
  example in a still running `OnNext` call.

In this repo I play with an alternate interface, that tries to circumvent these
two points.

## IAsyncObservable interface and contract

There are manly two interfaces involved, `IAsnycObservable`:

```csharp
public interface IAsyncObservable<out T>
{
    ValueTask SubscribeAsync(IAsyncObserver<T> observer);
}
```
it is (almost) obvious what it does and 

```csharp
public interface IAsyncObserver<in T>
{
    ValueTask OnSubscibeAsync(ICancelable cancelable);
    ValueTask OnNextAsync(T value);
    ValueTask OnCompletedAsync();
    ValueTask OnErrorAsync(Exception error);
    ValueTask OnFinallyAsync();
}
```

The interface may only be used sequentially. All methods must not be called
in parallel. The returned tasks have to be awaited before another or the same
method is allowed to be called. For an operator it is ok to rely on that
producer and consumer play nice. It is not required to defend the contract
against a broken producer or consumer. Operators should if possible pass
the returned tasks through, so that in an ideal case the task of the final
consumer is passed directly to the source. That has the consequence that
a task may be in a faulted state. Faulted tasks are explicitly allowed.
Consequently an operator that does more than passing exceptions through
via `OnErrorAsync()`, like `Catch` for example, has to actively protect
the upstream from faulted downstream tasks.


### `OnSubscribeAsync()`

The first method to be called is `OnSubscribeAsync()`. It passes a cancellation 
trigger to the downstream observer enabling it to stop further upstream production.
If an observer needs to acquire any resources, the `OnSubscribeAsync()` method 
would be a good place to do that. Some operator may trigger the cancellation
immediately like for example `Take(0)`. It is not mandatory for an operator to
react on the cancellation and an operator can choose to simply pass the upstream
cancelable through. But it is good practice to stop at least any further
propagation.

### `OnNextAsync()`, `OnCompletedAsync()` and `OnErrorAsync()`

The semantics of those three methods are very similar to the synchronous
`IObserver` contract. If an operator terminates a sequence, like for example
`Take`, it should cancel the upstream observer. If it is simply forwarding a
completion or an error it should not call `Dispose` of the upstream
`Cancelable`. 

### `OnFinallyAsync()`

The `OnFinallyAsync()` method should be called after `OnCompletedAsync()`, 
`OnErrorAsync()` or - in the case of a cancellation - `OnNextAsync()` are await.
An operator can dispose used resources here safely.

## Open questions

The contract may and will probably change. There are some open questions.

 - What is the meaning of the `SubscribeAsync()` returned task? Will it be
   the point after `OnSubscribeAsync()` or when the sequence finished?
   or will it depend on the actual source.
 - When using `await` should the tasks be awaited with `.ConfigureAwait(true)`?