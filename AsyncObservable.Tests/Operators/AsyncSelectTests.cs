using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;
using FluentAssertions.Extensions;
using System.Threading;

namespace Tests
{
    public class AsyncSelectTests
    {
        [Fact]
        public void ArgumentExceptions()
        {
            var obs1 = AsyncObservable.Range(0, 10);
            var obs2 = (IAsyncObservable<int>)null;
            Func<int, ValueTask<int>> selector = null;

            obs1.Invoking(o => o.Select(selector))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Select(i => new ValueTask<int>(i)))
                .Should().Throw<ArgumentNullException>();
            obs2.Invoking(o => o.Select(selector))
                .Should().Throw<ArgumentNullException>();
        }

        static async ValueTask<T> CreateTask<T>(T v)
        {
            await Task.Yield();
            return v;
        }

        [Fact]
        public async Task Select1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Select(i => CreateTask(9 - i))
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("9876543210C");
        }

        [Fact]
        public async Task SelectWithCa()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Select((i, ca) => CreateTask(9 - i))
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("9876543210C");
        }

        [Fact]
        public async Task SelectWithCa2()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Select(async (i, ca) => await CreateTask(9 - i))
                .Take(2)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("98C");
        }

        [Fact]
        public async Task IntermediateCancel()
        {
            var result = "";
            var scheduler = new TestAsyncScheduler();

            await scheduler.RunAsync(async () =>
            {
                using (var cts = new CancellationTokenSource())
                {
                    var t1 = AsyncObservable.Range(0, 10)
                        .Select(async (i, ca) =>
                        {
                            await scheduler.Delay(20.Seconds(), ca);
                            return i;
                        })
                        .SubscribeAsync(i => result += i, onCompleted: () => result += "C", ca: cts.Token);

                    await scheduler.Delay(50.Seconds(), default);
                    cts.Cancel();
                    await t1;
                }

                return Unit.Default;
            });

            result
                .Should().Be("01");
        }

        [Fact]
        public async Task Throw()
        {
            string result = "";
            Func<int, ValueTask<int>> selector = i => throw new NotImplementedException();

            await AsyncObservable.Range(0, 10)
                .Select(selector)
                .SubscribeAsync(i => result += i, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("E");
        }
    }
}
