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
    public class DropOnBackpressureTests
    {
        [Fact]
        public async Task ProduceSlowly()
        {
            var scheduler = new TestAsyncScheduler();

            var result = await scheduler.RunAsync(async () =>
            {
                return await scheduler.ProduceSlowly(
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds())
                    .DropOnBackpressure()
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
        public async Task ConsumeSlowly()
        {
            var scheduler = new TestAsyncScheduler();

            var result = await scheduler.RunAsync(async () =>
            {
                return await AsyncObservable.Range(0, 5)
                    .DropOnBackpressure()
                    .ConsumeSlowly(scheduler,
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds());
            });

            result
                .Should().Equal((0, 10.Seconds()));
        }

        [Fact]
        public async Task ProduceConsumeSlowly1()
        {
            var scheduler = new TestAsyncScheduler();

            var result = await scheduler.RunAsync(async () =>
            {
                return await scheduler.ProduceSlowly(
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds())
                    .DropOnBackpressure()
                    .ConsumeSlowly(scheduler,
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds());
            });

            result
                .Should().Equal(
                (0, 20.Seconds()),
                (1, 30.Seconds()),
                (2, 40.Seconds()),
                (3, 50.Seconds()),
                (4, 60.Seconds()));
        }

        [Fact]
        public async Task ProduceConsumeSlowly2()
        {
            var scheduler = new TestAsyncScheduler();

            var result = await scheduler.RunAsync(async () =>
            {
                return await scheduler.ProduceSlowly(
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds(),
                        10.Seconds())
                    .DropOnBackpressure()
                    .ConsumeSlowly(scheduler,
                        20.Seconds(),
                        20.Seconds(),
                        20.Seconds(),
                        20.Seconds(),
                        20.Seconds());
            });

            result
                .Should().Equal(
                (0, 30.Seconds()),
                (2, 50.Seconds()),
                (4, 70.Seconds()));
        }
    }
}
