using FluentAssertions;
using FluentAssertions.Extensions;
using Quinmars.AsyncObservable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class MergeTests
    {
        [Fact]
        public async Task SyncSingle()
        {
            var result = "";

            await new[]
            {
                AsyncObservable.Range(0, 10)
            }
                .ToAsyncObservable()
                .Merge()
                .SubscribeAsync(onNext: o => result += o, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task SyncMultiple()
        {
            var result = "";

            await new[]
            {
                AsyncObservable.Range(0, 5),
                AsyncObservable.Range(5, 5)
            }
                .ToAsyncObservable()
                .Merge()
                .SubscribeAsync(onNext: o => result += o, onError: ex => result += "E", onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task ProduceSlowlySingle()
        {
            var scheduler = new TestAsyncScheduler();

            var result = await scheduler.RunAsync(async () =>
            {
                var inner = scheduler.ProduceSlowly(
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds());

                return await new [] { inner }
                    .ToAsyncObservable()
                    .Merge()
                    .ConsumeFast(scheduler);
            });

            result
                .Should().Equal(
                (0, 10.Seconds()),
                (1, 20.Seconds()),
                (2, 30.Seconds()),
                (3, 40.Seconds()),
                (4, 50.Seconds()));
        }

        [Fact]
        public async Task ProduceSlowlyMultiple()
        {
            var scheduler = new TestAsyncScheduler();

            var result = await scheduler.RunAsync(async () =>
            {
                var inner1 = scheduler.ProduceSlowly(
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds());

                var inner2 = inner1
                        .Select(i => i + 10);

                return await new [] { inner1, inner2 }
                    .ToAsyncObservable()
                    .Merge()
                    .ConsumeFast(scheduler);
            });

            result
                .Should().Equal(
                (0, 10.Seconds()),
                (10, 10.Seconds()),
                (1, 20.Seconds()),
                (11, 20.Seconds()),
                (2, 30.Seconds()),
                (12, 30.Seconds()),
                (3, 40.Seconds()),
                (13, 40.Seconds()),
                (4, 50.Seconds()),
                (14, 50.Seconds()));
        }

        [Fact]
        public async Task ConsumeSlowlySingle()
        {
            var scheduler = new TestAsyncScheduler();

            var result = await scheduler.RunAsync(async () =>
            {
                var inner = AsyncObservable.Range(0, 5);

                return await new [] { inner }
                    .ToAsyncObservable()
                    .Merge()
                    .ConsumeSlowly(scheduler,
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds());
            });

            result
                .Should().Equal(
                (0, 10.Seconds()),
                (1, 20.Seconds()),
                (2, 30.Seconds()),
                (3, 40.Seconds()),
                (4, 50.Seconds()));
        }

        [Fact]
        public async Task ConsumeSlowlyMultiple()
        {
            var scheduler = new TestAsyncScheduler();

            var result = await scheduler.RunAsync(async () =>
            {
                var inner1 = AsyncObservable.Range(0, 5);

                var inner2 = inner1
                        .Select(i => i + 10);

                return await new [] { inner1, inner2 }
                    .ToAsyncObservable()
                    .Merge()
                    .ConsumeSlowly(scheduler,
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds());
            });

            result
                .Should().Equal(
                ( 0, 10.Seconds()),
                (10, 20.Seconds()),
                ( 1, 30.Seconds()),
                (11, 40.Seconds()),
                ( 2, 50.Seconds()),
                (12, 60.Seconds()),
                ( 3, 70.Seconds()),
                (13, 80.Seconds()),
                ( 4, 90.Seconds()),
                (14, 100.Seconds()));
        }
    }
}
