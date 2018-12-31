using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using System.Threading;

namespace Tests
{
    public class UsingTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            Func<IDisposable> funcR = null;
            Func<IDisposable, IAsyncObservable<int>> funcO= null;

            Action action;

            action = () => AsyncObservable.Using(funcR, _ => AsyncObservable.Empty<int>());
            action
                .Should().Throw<ArgumentNullException>();

            action = () => AsyncObservable.Using(() => new BooleanDisposable(), funcO);
            action
                .Should().Throw<ArgumentNullException>();

            action = () => AsyncObservable.Using(funcR, funcO);
            action
                .Should().Throw<ArgumentNullException>();
        }

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
        public async Task Using2()
        {
            var cts = new CancellationTokenSource();
            var result = "";
            var t = AsyncObservable.Using(() => Disposable.Create(() => result += "3"),
                _ => AsyncObservable.Using(() => Disposable.Create(() => result += "2"),
                    __ => AsyncObservable.Using(() => Disposable.Create(() => result += "1"),
                        ___ =>AsyncObservable.Never<Unit>())))
                .Finally(() => result += "4")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C", ca: cts.Token);

            cts.Cancel();

            await t;

            result
                .Should().Be("1234");
        }
    }
}
