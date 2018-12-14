using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class ToListAsyncObserver<T> : ToTaskAsyncObserver<T, List<T>>
    {
        public ToListAsyncObserver()
        {
            Value = new List<T>();
            HasValue = true;
        }

        public override ValueTask OnNextAsync(T value)
        {
            if (!IsCanceled)
                Value.Add(value);

            return default;
        }
    }
}
