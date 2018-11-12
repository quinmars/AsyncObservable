using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class SumTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obsInt32 = (IAsyncObservable<int>)null;
            var obsDouble = (IAsyncObservable<int>)null;

            obsInt32.Invoking(o => o.Sum())
                .Should().Throw<ArgumentNullException>();
            obsDouble.Invoking(o => o.Sum())
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Int32_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<int>()
                .Sum()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0C");
        }

        [Fact]
        public async Task Int32_Range()
        {
            int result = 0;

            await AsyncObservable.Range(0, 3)
                .Sum()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(0 + 1 + 2);
        }

        [Fact]
        public async Task Double_Empty()
        {
            double result = 0.0;

            await AsyncObservable.Empty<double>()
                .Sum()
                .SubscribeAsync(i => result = i);

            result
                .Should().Be(0.0);
        }

        [Fact]
        public async Task Double_Range()
        {
            double result = 0.0;

            await new[] { 0.0, 4.0, 13.0, 3.0}.ToAsyncObservable()
                .Sum()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(20.0);
        }
    }
}
