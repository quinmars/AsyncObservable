using Quinmars.AsyncObservable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    static class TestExtensions
    {
        public static IAsyncObservable<int> ProduceSlowly(this IAsyncScheduler scheduler, params TimeSpan[] delays)
        {
            return AsyncObservable.Range(0, delays.Length)
                .Select(async (i, ca) =>
                {
                    await scheduler.Delay(delays[i], ca).ConfigureAwait(false);
                    return i;
                });
        }

        public static async Task<IList<(T, TimeSpan)>> ConsumeFast<T>(this IAsyncObservable<T> obs, TestAsyncScheduler scheduler)
        {
            var list = new List<(T, TimeSpan)>();

            await obs.SubscribeAsync(i =>
            {
                list.Add((i, scheduler.EllapsedTime));
            }).ConfigureAwait(false);

            return list;
        }

        public static async Task<IList<(T, TimeSpan)>> ConsumeSlowly<T>(this IAsyncObservable<T> obs, TestAsyncScheduler scheduler, params TimeSpan[] delays)
        {
            var list = new List<(T, TimeSpan)>();
            int index = 0;

            await obs.SubscribeAsync(async i =>
            {
                await scheduler.Delay(delays[index], default).ConfigureAwait(false);
                list.Add((i, scheduler.EllapsedTime));
                index++;
            }).ConfigureAwait(false);

            return list;
        }
    }
}
