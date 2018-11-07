using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class DoTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;

            obs1.Invoking(o => o.Do(null))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Do(_ => { }))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Do(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Do1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Do(i => result += i)
                .Take(2)
                .SubscribeAsync(onCompleted: () => result += "C");

            result
                .Should().Be("01C");
        }

        [Fact]
        public async Task Do2()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Take(2)
                .Do(i => result += i)
                .SubscribeAsync(onCompleted: () => result += "C");

            result
                .Should().Be("01C");
        }

        [Fact]
        public async Task Do3()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Do(i =>
                {
                    result += i;

                    if (i == 2)
                        throw new Exception();
                })
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("0N1N2E");
        }
    }
}
