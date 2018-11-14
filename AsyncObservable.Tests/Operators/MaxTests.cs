using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class MaxTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obsInt32 = (IAsyncObservable<int>)null;
            var obsDouble = (IAsyncObservable<int>)null;

            obsInt32.Invoking(o => o.Max())
                .Should().Throw<ArgumentNullException>();
            obsDouble.Invoking(o => o.Max())
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Int32_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<int>()
                .Max()
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task Int32_Range()
        {
            int result = 0;

            await AsyncObservable.Range(0, 3)
                .Max()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(2);
        }

        [Fact]
        public async Task Int32_Random()
        {
            int result = 0;

            await new [] {4, 6, 1 -5, 3}.ToAsyncObservable()
                .Max()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(6);
        }

        [Fact]
        public async Task Double_Empty()
        {
            var result = "";

            await AsyncObservable.Empty<double>()
                .Max()
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task Double_Some()
        {
            double result = 0.0;

            await new[] { 0.0, 4.0, -13.0, 3.0}.ToAsyncObservable()
                .Max()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(4.0);
        }

        [Fact]
        public async Task Double_NaN()
        {
            double result = 0.0;

            await new[] { 0.0, Double.NaN, 4.0, -13.0, 3.0}.ToAsyncObservable()
                .Max()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(Double.NaN);
        }
    }
}
