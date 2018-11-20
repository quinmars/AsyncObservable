using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class Using<TResource, TResult> : IAsyncObservable<TResult> where TResource : IDisposable
    {
        readonly Func<TResource> _resourceFactory;
        readonly Func<TResource, IAsyncObservable<TResult>> _observableFactory;

        public Using(Func<TResource> resourceFactory, Func<TResource, IAsyncObservable<TResult>> observableFactory)
        {
            _resourceFactory = resourceFactory;
            _observableFactory = observableFactory;
        }

        public ValueTask SubscribeAsync(IAsyncObserver<TResult> observer)
        {
            IAsyncObservable<TResult> source;
            try
            {
                var resource = _resourceFactory();
                source = _observableFactory(resource).Finally(() => resource.Dispose());
            }
            catch (Exception ex)
            {
                source = AsyncObservable.Throw<TResult>(ex);
            }

            return source.SubscribeAsync(observer);
        }
    }
}
