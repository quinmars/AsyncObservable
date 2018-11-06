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
        public void Never1()
        {
            string result = "";

            var d = AsyncObservable.Never<int>()
                .Subscribe(i => result += i, ex => result += "E", () => result += "C");

            d.Dispose();
            result
                .Should().Be("");
        }
    }
}
