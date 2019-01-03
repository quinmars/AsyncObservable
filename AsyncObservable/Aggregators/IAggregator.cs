using System;
using System.Collections.Generic;
using System.Text;

namespace Quinmars.AsyncObservable
{
    interface IAggreagator<Tin, Tout>
    {
        void Init();
        bool HasValue { get; }
        Tout Value { get; }
        bool Add(Tin value);
    }
}
