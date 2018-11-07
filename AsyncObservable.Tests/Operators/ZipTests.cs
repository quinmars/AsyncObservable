using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class ZipTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;

            obs1.Invoking(o => o.Zip(obs2))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Zip(obs1))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Zip(obs2))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Zip1()
        {
            var result = "";
            var a = AsyncObservable.Range(0, 4);
            var b = AsyncObservable.Range(5, 4);

            await a.Zip(b)
                .SubscribeAsync(i => result += $"{i.Item1}{i.Item2}", ex => result += "E", () => result += "C");

            result
                .Should().Be("05162738C");
        }

        [Fact]
        public async Task Zip2()
        {
            var result = "";
            var a = AsyncObservable.Range(0, 5);
            var b = AsyncObservable.Range(5, 4);

            await a.Zip(b)
                .SubscribeAsync(i => result += $"{i.Item1}{i.Item2}", ex => result += "E", () => result += "C");

            result
                .Should().Be("05162738C");
        }

        [Fact]
        public async Task Zip3()
        {
            var result = "";
            var a = AsyncObservable.Range(0, 4);
            var b = AsyncObservable.Range(5, 5);

            await a.Zip(b)
                .SubscribeAsync(i => result += $"{i.Item1}{i.Item2}", ex => result += "E", () => result += "C");

            result
                .Should().Be("05162738C");
        }

        [Fact]
        public async Task Zip4()
        {
            var result = "";
            var a = AsyncObservable.Range(0, 4)
                .Do(i =>
                {
                    if (i == 1)
                        throw new Exception();
                });

            var b = AsyncObservable.Range(5, 4);

            await a.Zip(b)
                .SubscribeAsync(i => result += $"{i.Item1}{i.Item2}", ex => result += "E", () => result += "C");

            result
                .Should().Be("05E");
        }
    }
}
