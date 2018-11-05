using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Reactive.Linq;

namespace Tests
{
    public class FromObservableTests
    {
        [Fact]
        public async Task Empty()
        {
            string result = "";

            var d = await Observable
                .Empty<int>()
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("C");
        }

        //[Fact]
        public async Task Never()
        {
            string result = "";

            var d = await Observable.Never<int>()
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            d.Dispose();

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task Return()
        {
            string result = "";

            var d = await Observable
                .Return(1)
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("1C");
        }

        [Fact]
        public async Task Range()
        {
            string result = "";

            var d = await Observable
                .Range(0, 4)
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("0123C");
        }

        [Fact]
        public async Task Throw1()
        {
            string result = "";

            var d = await Observable
                .Throw<int>(new Exception())
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task Throw2()
        {
            string result = "";

            var d = await Observable
                .Range(0, 4)
                .Do(i =>
                {
                    if (i == 2)
                        throw new Exception();
                })
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("01E");
        }
       
    }
}
