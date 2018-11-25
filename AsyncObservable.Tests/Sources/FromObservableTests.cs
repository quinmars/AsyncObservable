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
        public void ArgumentExceptions()
        {
            var obs = (IObservable<int>)null;

            obs.Invoking(o => o.ToAsyncObservable())
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Empty()
        {
            string result = "";

            await Observable
                .Empty<int>()
                .ToAsyncObservable()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("C");
        }

        //[Fact]
        public void Never()
        {
            string result = "";

            var d = Observable.Never<int>()
                .ToAsyncObservable()
                .Subscribe(i => result += i, ex => result += "E", () => result += "C");

            d.Dispose();

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task Return()
        {
            string result = "";

            await Observable
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

            await Observable
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

            await Observable
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

        await Observable
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
