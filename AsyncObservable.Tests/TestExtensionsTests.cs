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
    public class TestExtensionsTests
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
        public async Task ProduceConsumeSlowly()
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
                (1, 40.Seconds()),
                (2, 60.Seconds()),
                (3, 80.Seconds()),
                (4, 100.Seconds()));
        }
    }
}
