using System;
using System.Collections.Generic;
using System.Text;

namespace Quinmars.AsyncObservable
{
    struct SumIntAggregator : IAggreagator<int, int>
    {
        bool _first;
        public void Init() => _first = true;
        public bool HasValue => true;
        public int Value { get; set; }

        public bool Add(int value)
        {
            var first = _first;
            _first = false;
            checked
            {
                Value += value;
            }

            return first | value != 0;
        }
    }

    struct SumDoubleAggregator : IAggreagator<double, double>
    {
        bool _first;
        public void Init() => _first = true;
        public bool HasValue => true;
        public double Value { get; set; }

        public bool Add(double value)
        {
            var first = _first;
            _first = false;

            Value += value;

            return first | value != 0.0;
        }
    }
}
