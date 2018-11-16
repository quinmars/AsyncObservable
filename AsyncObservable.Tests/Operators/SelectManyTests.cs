using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class SelectManyTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = default(IAsyncObservable<int>);
            Func<int, IEnumerable<int>> selector = null;

            obs1.Invoking(o => o.SelectMany(selector))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.SelectMany(i => new[] { i }))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.SelectMany(selector))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Select1()
        {
            string result = "";

            await AsyncObservable.Return(Unit.Default)
                .SelectMany(_ => Enumerable.Range(0, 10))
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task Select2()
        {
            string result = "";

            await AsyncObservable.Range(0, 3)
                .SelectMany(i => Enumerable.Range(i*2, 2))
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("012345C");
        }

        [Fact]
        public async Task Take3()
        {
            string result = "";

            await AsyncObservable.Range(0, 3)
                .SelectMany(i => Enumerable.Range(i*2, 2))
                .Do(_ => result += "_")
                .Take(3)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("_0_1_2C");
        }

        [Fact]
        public async Task Take4()
        {
            string result = "";

            await AsyncObservable.Range(0, 3)
                .SelectMany(i => Enumerable.Range(i*2, 2))
                .Do(_ => result += "_")
                .Take(4)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("_0_1_2_3C");
        }

        [Fact]
        public async Task Take5()
        {
            string result = "";

            await AsyncObservable.Range(0, 3)
                .SelectMany(i => Enumerable.Range(i*2, 2))
                .Do(_ => result += "_")
                .Take(5)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("_0_1_2_3_4C");
        }
    }
}
