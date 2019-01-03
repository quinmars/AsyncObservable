using System;
using System.Collections.Generic;
using System.Text;

namespace Quinmars.AsyncObservable
{
    struct SumIntAggregator : IAggreagator<int, int>
    {
        public void Init() { }
        public bool HasValue => true;
        public int Value { get; set; }

        public bool Add(int value)
        {
            checked
            {
                Value += value;
            }

            return value != 0;
        }
    }

    struct SumDoubleAggregator : IAggreagator<double, double>
    {
        public void Init() { }
        public bool HasValue => true;
        public double Value { get; set; }

        public bool Add(double value)
        {
            Value += value;
            return value != 0.0;
        }
    }
}
