using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class AllAsyncObserver<T> : ToTaskAsyncObserver<T, bool>
    {
        readonly Func<T, bool> _predicate;

        public AllAsyncObserver(Func<T, bool> predicate)
        {
            Value = true;
            HasValue = true;
            _predicate = predicate;
        }

        public override ValueTask OnNextAsync(T value)
        {
            if (!IsCanceled && !_predicate(value))
            {
                Dispose();
                Value = false;
            }
            return default;
        }
    }
}
