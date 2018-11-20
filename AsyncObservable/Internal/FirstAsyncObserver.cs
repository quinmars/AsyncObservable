using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quinmars.AsyncObservable
{
    class FirstAsyncObserver<T> : ToTaskAsyncObserver<T>
    {
        public override ValueTask OnNextAsync(T value)
        {
            if (!IsCanceled)
            {
                Dispose();
                Value = value;
                HasValue = true;
            }
            return default;
        }
    }
}
