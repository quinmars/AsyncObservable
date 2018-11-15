using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Collections.Generic;

namespace Tests
{
    public class ConcatTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = default(IAsyncObservable<IAsyncObservable<int>>);
            var obs2 = default(IAsyncObservable<int>[]);
            var obs3 = default(IEnumerable<IAsyncObservable<int>>);

            obs1.Invoking(o => o.Concat())
               .Should().Throw<ArgumentNullException>();

            obs2.Invoking(o => AsyncObservable.Concat(o))
               .Should().Throw<ArgumentNullException>();

            obs3.Invoking(o => o.Concat())
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
        public async Task ThrowingItems()
        {
            string result = "";

            var seq = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Throw<int>(new Exception()),
                AsyncObservable.Throw<int>(new Exception())
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

        [Fact]
        public async Task Params0()
        {
            string result = "";

            await AsyncObservable
                .Concat(new IAsyncObservable<int>[] { })
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("CF");
        }

        [Fact]
        public async Task Params1()
        {
            string result = "";

            var obs1 = AsyncObservable.Range(0, 3).Finally(() => result += ",");

            await AsyncObservable
                .Concat(obs1)
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,CF");
        }

        [Fact]
        public async Task Params2()
        {
            string result = "";

            var obs1 = AsyncObservable.Range(0, 3).Finally(() => result += ",");
            var obs2 = AsyncObservable.Range(3, 2).Finally(() => result += ",");

            await AsyncObservable
                .Concat(obs1, obs2)
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,34,CF");
        }

        [Fact]
        public async Task Params3()
        {
            string result = "";

            var obs1 = AsyncObservable.Range(0, 3).Finally(() => result += ",");
            var obs2 = AsyncObservable.Range(3, 2).Finally(() => result += ",");
            var obs3 = AsyncObservable.Range(5, 3).Finally(() => result += ",");

            await AsyncObservable
                .Concat(obs1, obs2, obs3)
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,34,567,CF");
        }

        [Fact]
        public async Task Params3Error1()
        {
            string result = "";

            var obs1 = AsyncObservable.Range(0, 3).Finally(() => result += ",");
            var obs2 = AsyncObservable.Throw<int>(new Exception());
            var obs3 = AsyncObservable.Range(5, 3).Finally(() => result += ",");

            await AsyncObservable
                .Concat(obs1, obs2, obs3)
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }

        [Fact]
        public async Task Params3Error2()
        {
            string result = "";

            var obs1 = AsyncObservable.Range(0, 3).Finally(() => result += ",");
            var obs2 = AsyncObservable.Throw<int>(new Exception());
            var obs3 = AsyncObservable.Throw<int>(new Exception());

            await AsyncObservable
                .Concat(obs1, obs2, obs3)
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }

        [Fact]
        public async Task Params3Null1()
        {
            string result = "";

            var obs1 = AsyncObservable.Range(0, 3).Finally(() => result += ",");
            var obs2 = default(IAsyncObservable<int>);
            var obs3 = AsyncObservable.Range(5, 3).Finally(() => result += ",");

            await AsyncObservable
                .Concat(obs1, obs2, obs3)
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }

        [Fact]
        public async Task Params3Null2()
        {
            string result = "";

            var obs1 = AsyncObservable.Range(0, 3).Finally(() => result += ",");
            var obs2 = default(IAsyncObservable<int>);
            var obs3 = default(IAsyncObservable<int>);

            await AsyncObservable
                .Concat(obs1, obs2, obs3)
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }

        [Fact]
        public async Task Enum1()
        {
            string result = "";

            var obs = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ",")
            };

            await obs
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,CF");
        }

        [Fact]
        public async Task Enum2()
        {
            string result = "";

            var obs = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Range(3, 2).Finally(() => result += ",")
            };

            await obs
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,34,CF");
        }

        [Fact]
        public async Task Enum3()
        {
            string result = "";

            var obs = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Range(3, 2).Finally(() => result += ","),
                AsyncObservable.Range(5, 3).Finally(() => result += ",")
            };

            await obs
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,34,567,CF");
        }

        [Fact]
        public async Task Enum3Error1()
        {
            string result = "";

            var obs = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Throw<int>(new Exception()),
                AsyncObservable.Range(5, 3).Finally(() => result += ",")
            };

            await obs
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }

        [Fact]
        public async Task Enum3Error2()
        {
            string result = "";

            var obs = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                AsyncObservable.Throw<int>(new Exception()),
                AsyncObservable.Throw<int>(new Exception())
            };

            await obs
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }

        [Fact]
        public async Task Enum3Null1()
        {
            string result = "";

            var obs = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                default(IAsyncObservable<int>),
                AsyncObservable.Range(5, 3).Finally(() => result += ",")
            };

            await obs
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }

        [Fact]
        public async Task Enum3Null2()
        {
            string result = "";

            var obs = new[]
            {
                AsyncObservable.Range(0, 3).Finally(() => result += ","),
                default(IAsyncObservable<int>),
                default(IAsyncObservable<int>)
            };

            await obs
                .Concat()
                .Finally(() => result += "F")
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("012,EF");
        }
    }
}
