using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class LastAsyncObserver<T> : ToTaskAsyncObserver<T, T>
    {
        public override ValueTask OnNextAsync(T value)
        {
            if (!IsCanceled)
            {
                Value = value;
                HasValue = true;
            }
            return default;
        }
    }
}
