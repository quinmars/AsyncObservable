using System;
using System.Collections.Generic;
using System.Text;

namespace Quinmars.AsyncObservable
{
    struct MaxIntAggregator : IAggreagator<int, int>
    {
        public void Init() { }
        public bool HasValue { get; set; }
        public int Value { get; set; }

        public bool Add(int value)
        {
            if (HasValue)
            {
                if (value > Value)
                {
                    Value = value;
                    return true;
                }
            }
            else
            {
                HasValue = true;
                Value = value;
                return true;
            };

            return false;
        }
    }

    struct MaxIntNullableAggregator : IAggreagator<int?, int?>
    {
        bool hasNoneNull;
        bool first;

        public void Init()
        {
            hasNoneNull = false;
            first = true;
            Value = null;
        }

        public bool HasValue => true;
        public int? Value { get; set; }

        public bool Add(int? value)
        {
            if (!hasNoneNull)
            {
                var temp = hasNoneNull;
                Value = value;
                if (value.HasValue)
                    hasNoneNull = true;

                if (first)
                {
                    first = false;
                    return true;
                }

                return temp != hasNoneNull;
            }
            if (value > Value)
            {
                Value = value;
                return true;
            }

            return false;
        }
    }

    struct MaxLongAggregator : IAggreagator<long, long>
    {
        public void Init() { }
        public bool HasValue { get; set; }
        public long Value { get; set; }

        public bool Add(long value)
        {
            if (HasValue)
            {
                if (value > Value)
                {
                    Value = value;
                    return true;
                }
            }
            else
            {
                HasValue = true;
                Value = value;
                return true;
            };

            return false;
        }
    }

    struct MaxLongNullableAggregator : IAggreagator<long?, long?>
    {
        public void Init()
        {
            Value = null;
        }

        public bool HasValue => true;
        public long? Value { get; set; }

        public bool Add(long? value)
        {
            if (value > Value)
            {
                Value = value;
                return true;
            }

            return false;
        }
    }


    struct MaxDoubleAggregator : IAggreagator<double, double>
    {
        public void Init() { }
        public bool HasValue { get; set; }
        public double Value { get; set; }

        public bool Add(double value)
        {
            if (HasValue)
            {
                // if value is NaN Value should also become NaN
                if (value > Value || double.IsNaN(value))
                {
                    Value = value;
                    return true;
                }
            }
            else
            {
                HasValue = true;
                Value = value;
                return true;
            };

            return false;
        }
    }
}
