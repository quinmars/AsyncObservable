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
        public async Task Finally1()
        {
            string result = "";

            var d = await AsyncObservable.Never<int>()
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            d.Dispose();
            result
                .Should().Be("12");
        }

        [Fact]
        public async Task Finally2()
        {
            string result = "";

            var d = await AsyncObservable.Never<int>()
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

            var d = await AsyncObservable.Empty<int>()
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
