using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class FinallyTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;

            obs1.Invoking(o => o.Finally(null))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Finally(() => { }))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Finally(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Finally1()
        {
            string result = "";

            var d = AsyncObservable.Never<int>()
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .Subscribe(i => result += "N", ex => result += "E", () => result += "C");

            d.Dispose();
            result
                .Should().Be("12");
        }

        [Fact]
        public void Finally2()
        {
            string result = "";

            AsyncObservable.Never<int>()
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("");
        }

        [Fact]
        public async Task Finally3()
        {
            string result = "";

            await AsyncObservable.Empty<int>()
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("C12");
        }

        [Fact]
        public async Task Finally4()
        {
            string result = "";

            await AsyncObservable.Return(1)
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("NC12");
        }

        [Fact]
        public async Task Finally5()
        {
            string result = "";

            await AsyncObservable.Throw<int>(new Exception())
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("E12");
        }
    }
}
