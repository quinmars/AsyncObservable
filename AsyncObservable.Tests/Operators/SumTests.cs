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
        public async Task Int_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<int>()
                .Sum()
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0C");
        }

        [Fact]
        public async Task Int_Range()
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
        public async Task Int_Intermediate_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<int>()
                .Sum(intermediateResults: true)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task Int_Intermediate_Range()
        {
            var result = await AsyncObservable.Range(0, 3)
                .Sum(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(0, 1, 3);
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

        [Fact]
        public async Task Double_Intermediate_Empty()
        {
            double result = Double.NaN;

            await AsyncObservable.Empty<double>()
                .Sum(intermediateResults: true)
                .SubscribeAsync(i => result = i);

            result
                .Should().Be(Double.NaN);
        }

        [Fact]
        public async Task Double_Intermediate_Range()
        {
            var result = await new[] { 0.0, 4.0, 13.0, 3.0}.ToAsyncObservable()
                .Sum(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(0.0, 4.0, 17.0, 20.0);
        }
    }
}
