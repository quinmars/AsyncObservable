using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class UsingTests
    {
        [Fact]
        public async Task Using1()
        {
            var result = "";
            await AsyncObservable.Using(() => Disposable.Create(() => result += "3"),
                _ => AsyncObservable.Using(() => Disposable.Create(() => result += "2"),
                    __ => AsyncObservable.Using(() => Disposable.Create(() => result += "1"),
                        ___ =>AsyncObservable.Return(Unit.Default))))
                .Finally(() => result += "4")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("NC1234");
        }

        [Fact]
        public void Using2()
        {
            var result = "";
            var d = AsyncObservable.Using(() => Disposable.Create(() => result += "3"),
                _ => AsyncObservable.Using(() => Disposable.Create(() => result += "2"),
                    __ => AsyncObservable.Using(() => Disposable.Create(() => result += "1"),
                        ___ =>AsyncObservable.Never<Unit>())))
                .Finally(() => result += "4")
                .Subscribe(i => result += "N", ex => result += "E", () => result += "C");

            d.Dispose();

            result
                .Should().Be("1234");
        }
    }
}
