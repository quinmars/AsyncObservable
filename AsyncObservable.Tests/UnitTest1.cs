using System;
using Xunit;
using Quinmars.AsyncObservable;
using System.Threading.Tasks;
using FluentAssertions;
using System.Reactive.Disposables;
using System.Reactive;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Range1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("0123456789C");
        }

        [Fact]
        public async Task Select1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Select(i => 9 - i)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("9876543210C");
        }

        [Fact]
        public async Task Take0()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Take(0)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task Take1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Take(2)
                .SubscribeAsync(i => result += i, onCompleted: () => result += "C");

            result
                .Should().Be("01C");
        }

        [Fact]
        public async Task Do1()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Do(i => result += i)
                .Take(2)
                .SubscribeAsync(onCompleted: () => result += "C");

            result
                .Should().Be("01C");
        }

        [Fact]
        public async Task Do2()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Take(2)
                .Do(i => result += i)
                .SubscribeAsync(onCompleted: () => result += "C");

            result
                .Should().Be("01C");
        }

        [Fact]
        public async Task Do3()
        {
            string result = "";

            await AsyncObservable.Range(0, 10)
                .Do(i =>
                {
                    result += i;

                    if (i == 2)
                        throw new Exception();
                })
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("0N1N2E");
        }

        [Fact]
        public async Task Never1()
        {
            string result = "";

            var d = await AsyncObservable.Never<int>()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            await d.DisposeAsync();
            result
                .Should().Be("");
        }

        [Fact]
        public async Task Empty1()
        {
            string result = "";

            var d = await AsyncObservable.Empty<int>()
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("C");
        }

        [Fact]
        public async Task Return1()
        {
            string result = "";

            var d = await AsyncObservable.Return(1)
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("1C");
        }

        [Fact]
        public async Task Throw1()
        {
            string result = "";

            var d = await AsyncObservable.Throw<int>(new Exception())
                .SubscribeAsync(i => result += i, ex => result += "E", () => result += "C");

            result
                .Should().Be("E");
        }

        [Fact]
        public async Task Finally1()
        {
            string result = "";

            var d = await AsyncObservable.Never<int>()
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            await d.DisposeAsync();
            result
                .Should().Be("12");
        }

        [Fact]
        public async Task Finally2()
        {
            string result = "";

            var d = await AsyncObservable.Never<int>()
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("");
        }

        [Fact]
        public async Task Finally3()
        {
            string result = "";

            var d = await AsyncObservable.Empty<int>()
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("12C");
        }

        [Fact]
        public async Task Finally4()
        {
            string result = "";

            await AsyncObservable.Return(1)
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("N12C");
        }

        [Fact]
        public async Task Finally5()
        {
            string result = "";

            await AsyncObservable.Throw<int>(new Exception())
                .Finally(() => result += "1")
                .Finally(() => result += "2")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            result
                .Should().Be("12E");
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
                .Should().Be("N1234C");
        }

        [Fact]
        public async Task Using2()
        {
            var result = "";
            var d = await AsyncObservable.Using(() => Disposable.Create(() => result += "3"),
                _ => AsyncObservable.Using(() => Disposable.Create(() => result += "2"),
                    __ => AsyncObservable.Using(() => Disposable.Create(() => result += "1"),
                        ___ =>AsyncObservable.Never<Unit>())))
                .Finally(() => result += "4")
                .SubscribeAsync(i => result += "N", ex => result += "E", () => result += "C");

            await d.DisposeAsync();

            result
                .Should().Be("1234");
        }

        [Fact]
        public async Task Zip1()
        {
            var result = "";
            var a = AsyncObservable.Range(0, 4);
            var b = AsyncObservable.Range(5, 4);

            await a.Zip(b)
                .SubscribeAsync(i => result += $"{i.Item1}{i.Item2}", ex => result += "E", () => result += "C");

            result
                .Should().Be("05162738C");
        }

        [Fact]
        public async Task Zip2()
        {
            var result = "";
            var a = AsyncObservable.Range(0, 5);
            var b = AsyncObservable.Range(5, 4);

            await a.Zip(b)
                .SubscribeAsync(i => result += $"{i.Item1}{i.Item2}", ex => result += "E", () => result += "C");

            result
                .Should().Be("05162738C");
        }

        [Fact]
        public async Task Zip3()
        {
            var result = "";
            var a = AsyncObservable.Range(0, 4);
            var b = AsyncObservable.Range(5, 5);

            await a.Zip(b)
                .SubscribeAsync(i => result += $"{i.Item1}{i.Item2}", ex => result += "E", () => result += "C");

            result
                .Should().Be("05162738C");
        }

        [Fact]
        public async Task Zip4()
        {
            var result = "";
            var a = AsyncObservable.Range(0, 4)
                .Do(i =>
                {
                    if (i == 1)
                        throw new Exception();
                });

            var b = AsyncObservable.Range(5, 4);

            await a.Zip(b)
                .SubscribeAsync(i => result += $"{i.Item1}{i.Item2}", ex => result += "E", () => result += "C");

            result
                .Should().Be("05E");
        }
    }
}
