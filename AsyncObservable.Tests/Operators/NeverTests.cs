using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class NeverTests
    {
        [Fact]
        public async Task Never1()
        {
            string result = "";

            var d = await AsyncObservable.Never<int>()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            d.Dispose();
            result
                .Should().Be("");
        }
    }
}
