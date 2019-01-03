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

        /*
         * INT
         */
        [Fact]
        public async Task Int_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<int>()
                .Max()
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task Int_Range()
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
        public async Task Int_Random()
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
        public async Task Int_Intermediate_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<int>()
                .Max(intermediateResults: true)
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task Int_Intermediate_Range()
        {
            var result = await AsyncObservable.Range(0, 3)
                .Max(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(0, 1, 2);
        }

        [Fact]
        public async Task Int_Intermediate_Random()
        {
            var result = await new [] {4, 6, 1 -5, 3}.ToAsyncObservable()
                .Max(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(4, 6);
        }

        [Fact]
        public async Task Int_Nullable_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<int?>()
                .Max()
                .SubscribeAsync(i => result += i == null ? "null" : i.ToString(), onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("nullC");
        }

        [Fact]
        public async Task Int_Nullable_Range()
        {
            int? result = null;

            await AsyncObservable.Range(0, 3)
                .Select(i => (int?)i)
                .Max()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(2);
        }

        [Fact]
        public async Task Int_Nullable_Random()
        {
            int? result = 0;

            await new int?[] {null, 4, 6, null, 1 -5, 3}.ToAsyncObservable()
                .Max()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(6);
        }

        [Fact]
        public async Task Int_Nullable_Intermediate_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<int?>()
                .Max(intermediateResults: true)
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task Int_Nullable_Intermediate_Range()
        {
            var result = await AsyncObservable.Range(0, 3)
                .Select(i => (int?)i)
                .Max(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(0, 1, 2);
        }

        [Fact]
        public async Task Int_Nullable_Intermediate_Random()
        {
            var result = await new int?[] {null, 4, 6, null, 1 -5, 3}.ToAsyncObservable()
                .Max(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(null, 4, 6);
        }

        /*
         * LONG
         */
        [Fact]
        public async Task Long_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<long>()
                .Max()
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task Long_Range()
        {
            long result = 0;

            await AsyncObservable.Range(0, 3)
                .Select(i => (long)i)
                .Max()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(2);
        }

        [Fact]
        public async Task Long_Random()
        {
            long result = 0;

            await new [] {4L, 6, 1 -5, 3}.ToAsyncObservable()
                .Max()
                .SubscribeAsync(i => result = i);

            result
                .Should()
                .Be(6);
        }

        [Fact]
        public async Task Long_Intermediate_Empty()
        {
            string result = "";

            await AsyncObservable.Empty<long>()
                .Max(intermediateResults: true)
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task Long_Intermediate_Range()
        {
            var result = await AsyncObservable.Range(0, 3)
                .Select(i => (long)i)
                .Max(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(0L, 1L, 2L);
        }

        [Fact]
        public async Task Long_Intermediate_Random()
        {
            var result = await new [] {4L, 6, 1 -5, 3}.ToAsyncObservable()
                .Max(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(4, 6);
        }

        /*
         * DOUBLE
         */
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

        [Fact]
        public async Task Double_Intermediate_Empty()
        {
            var result = "";

            await AsyncObservable.Empty<double>()
                .Max(intermediateResults: true)
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task Double_Intermediate_Some()
        {
            var result = await new[] { 0.0, 4.0, -13.0, 3.0}.ToAsyncObservable()
                .Max(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(0.0, 4.0);
        }

        [Fact]
        public async Task Double_Intermediate_NaN()
        {
            var result = await new[] { 0.0, Double.NaN, 4.0, -13.0, 3.0}.ToAsyncObservable()
                .Max(intermediateResults: true)
                .ToListAsync();

            result
                .Should()
                .BeEquivalentTo(0.0, Double.NaN);
        }
    }
}
