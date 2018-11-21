using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class AnyAsyncObserver<T> : ToTaskAsyncObserver<T, bool>
    {
        public AnyAsyncObserver()
        {
            Value = false;
            HasValue = true;
        }

        public override ValueTask OnNextAsync(T value)
        {
            if (!IsCanceled)
            {
                Dispose();
                Value = true;
            }
            return default;
        }
    }

    class AnyPredicateAsyncObserver<T> : ToTaskAsyncObserver<T, bool>
    {
        readonly Func<T, bool> _predicate;

        public AnyPredicateAsyncObserver(Func<T, bool> predicate)
        {
            Value = false;
            HasValue = true;
            _predicate = predicate;
        }

        public override ValueTask OnNextAsync(T value)
        {
            if (!IsCanceled && _predicate(value))
            {
                Dispose();
                Value = true;
            }
            return default;
        }
    }
}
