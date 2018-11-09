using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class ConcatTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs = (IAsyncObservable<IAsyncObservable<int>>)null;

            obs.Invoking(o => o.Concat())
               .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Single()
        {
            string result = "";

            var range = AsyncObservable.Range(0, 3);

            await AsyncObservable.Return(range.Finally(() => result += "f"))
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012fCF");
        }

        [Fact]
        public async Task Multiple()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Range(3, 2).Finally(() => result += ","),
                AsyncObservable.Range(5, 3).Finally(() => result += ",")
            }.ToAsyncObservable();

            await seq
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,34,567,CF");
        }

        [Fact]
        public async Task NullItem()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                null,
                AsyncObservable.Range(5, 3).Finally(() => result += ",")
            }.ToAsyncObservable();

            await seq
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }

        [Fact]
        public async Task ThrowingItem()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Throw<int>(new Exception()),
                AsyncObservable.Range(5, 3).Finally(() => result += ",")
            }.ToAsyncObservable();

            await seq
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }

        [Fact]
        public async Task Take()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Do(_ => result += "_").Finally(() => result += ","),
                AsyncObservable.Range(3, 2).Do(_ => result += "_").Finally(() => result += ","),
                AsyncObservable.Range(5, 3).Do(_ => result += "_").Finally(() => result += ",")
            }.ToAsyncObservable();

            await seq
                .Concat()
                .Take(4)
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("_0_1_2,_3C,F");
        }
    }
}
