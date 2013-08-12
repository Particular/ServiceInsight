using System;
using Machine.Specifications;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using Particular.ServiceInsight.Desktop.ExtensionMethods;

namespace NServiceBus.Profiler.Tests.TimeSpans
{
    [Subject("elapsed time")]
    public class when_converting_milliseconds_to_elapsed_time
    {
        protected static TimeSpan Time;
        protected static string Elapsed;

        Establish context = () => Time = new TimeSpan(0, 0, 0, 0, 50);

        Because of = () => Elapsed = Time.GetElapsedTime();

        It should_contain_only_the_milliseconds = () => Elapsed.ShouldEqual("50ms");
    }

    [Subject("elapsed time")]
    public class when_converting_seconds_and_milliseconds_to_elapsed_time
    {
        protected static TimeSpan Time;
        protected static string Elapsed;

        Establish context = () => Time = new TimeSpan(0, 0, 0, 3, 50);

        Because of = () => Elapsed = Time.GetElapsedTime();

        It should_contain_only_the_milliseconds = () => Elapsed.ShouldEqual("3s");
    }

    [Subject("elapsed time")]
    public class when_converting_seconds_to_elapsed_time
    {
        protected static TimeSpan Time;
        protected static string Elapsed;

        Establish context = () => Time = new TimeSpan(0, 0, 0, 3, 0);

        Because of = () => Elapsed = Time.GetElapsedTime();

        It should_contain_only_the_milliseconds = () => Elapsed.ShouldEqual("3s");
    }

    [Subject("elapsed time")]
    public class when_converting_minutes_and_seconds_to_elapsed_time
    {
        protected static TimeSpan Time;
        protected static string Elapsed;

        Establish context = () => Time = new TimeSpan(0, 0, 2, 30, 0);

        Because of = () => Elapsed = Time.GetElapsedTime();

        It should_contain_only_the_milliseconds = () => Elapsed.ShouldEqual("2m 30s");
    }

    [Subject("elapsed time")]
    public class when_converting_hours_and_minutes_and_seconds_to_elapsed_time
    {
        protected static TimeSpan Time;
        protected static string Elapsed;

        Establish context = () => Time = new TimeSpan(0, 1, 2, 30, 0);

        Because of = () => Elapsed = Time.GetElapsedTime();

        It should_contain_only_the_milliseconds = () => Elapsed.ShouldEqual("1h 2m 30s");
    }
}