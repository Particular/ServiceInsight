using System;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class ElapsedTimeTests
    {
        [Test]
        public void should_convert_milliseconds_to_elapsed_time()
        {
            var time = new TimeSpan(0, 0, 0, 0, 50);
            var elapsed = time.GetElapsedTime();
            elapsed.ShouldBe("50ms");
        }

        [Test]
        public void should_convert_seconds_and_milliseconds_to_elapsed_time()
        {
            var time = new TimeSpan(0, 0, 0, 3, 50);
            var elapsed = time.GetElapsedTime();
            elapsed.ShouldBe("3s");
        }

        [Test]
        public void should_convert_seconds_to_elapsed_time()
        {
            var time = new TimeSpan(0, 0, 0, 3, 0);
            var elapsed = time.GetElapsedTime();

            elapsed.ShouldBe("3s");
        }

        [Test]
        public void should_convert_minutes_and_seconds_to_elapsed_time()
        {
            var time = new TimeSpan(0, 0, 2, 30, 0);
            var elapsed = time.GetElapsedTime();
            elapsed.ShouldBe("2m 30s");
        }

        [Test]
        public void should_convert_hours_and_minutes_and_seconds_to_elapsed_time()
        {
            var time = new TimeSpan(0, 1, 2, 30, 0);
            var elapsed = time.GetElapsedTime();
            elapsed.ShouldBe("1h 2m 30s");
        }
    }
}