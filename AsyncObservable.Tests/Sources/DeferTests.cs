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
    public class DeferTests
    {
        [Fact]
        public void WithoutSubscription()
        {
            var called = 0;

            AsyncObservable.Defer(() =>
                {
                    Interlocked.Increment(ref called);
                    return AsyncObservable.Return(1);
                });

            called
                .Should().Be(0);
        }

        [Fact]
        public async Task WithSubscription()
        {
            var result = "";
            var called = 0;

            await AsyncObservable.Defer(() =>
                {
                    Interlocked.Increment(ref called);
                    return AsyncObservable.Return(1);
                })
                .SubscribeAsync(o => result += o, ex => result += "E", () => result += "C");

            called
                .Should().Be(1);

            result
                .Should().Be("1C");
        }

        [Fact]
        public async Task WithException()
        {
            var result = "";
            var called = 0;

            await AsyncObservable.Defer(delegate()
                {
                    Interlocked.Increment(ref called);
                    throw new NotImplementedException();
                    return AsyncObservable.Return(1);
                })
                .SubscribeAsync(o => result += o, ex => result += "E", () => result += "C");

            called
                .Should().Be(1);

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task WithException2()
        {
            var result = default(Exception);

            await AsyncObservable.Defer(delegate()
                {
                    throw new NotImplementedException();
                    return AsyncObservable.Return(1);
                })
                .SubscribeAsync(onError: ex => result = ex);

            result
                .Should().BeOfType<NotImplementedException>();
        }

        [Fact]
        public void AsyncWithoutSubscription()
        {
            var called = 0;

            AsyncObservable.Defer(() =>
            {
                Interlocked.Increment(ref called);
                return new ValueTask<IAsyncObservable<int>>(AsyncObservable.Return(1));
            });

            called
                .Should().Be(0);
        }

        [Fact]
        public async Task AsyncWithSubscription()
        {
            var result = "";
            var called = 0;

            await AsyncObservable.Defer(() =>
            {
                Interlocked.Increment(ref called);
                return new ValueTask<IAsyncObservable<int>>(AsyncObservable.Return(1));
            })
                .SubscribeAsync(o => result += o, ex => result += "E", () => result += "C");

            called
                .Should().Be(1);

            result
                .Should().Be("1C");
        }

        [Fact]
        public async Task AsyncWithException()
        {
            var result = "";
            var called = 0;

            await AsyncObservable.Defer(delegate ()
            {
                Interlocked.Increment(ref called);
                throw new NotImplementedException();
                return new ValueTask<IAsyncObservable<int>>(AsyncObservable.Return(1));
            })
                .SubscribeAsync(o => result += o, ex => result += "E", () => result += "C");

            called
                .Should().Be(1);

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task AsyncWithException2()
        {
            var result = default(Exception);

            await AsyncObservable.Defer(delegate ()
            {
                throw new NotImplementedException();
                return new ValueTask<IAsyncObservable<int>>(AsyncObservable.Return(1));
            })
                .SubscribeAsync(onError: ex => result = ex);

            result
                .Should().BeOfType<NotImplementedException>();
        }
    }
}
