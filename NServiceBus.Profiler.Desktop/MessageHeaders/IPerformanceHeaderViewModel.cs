using System;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IPerformanceHeaderViewModel : IHeaderInfoViewModel
    {
        DateTime? TimeSent { get; }
        DateTime? ProcessingStarted { get; }
        DateTime? ProcessingEnded { get; }
        string ProcessingTime { get; }
        string DeliveryTime { get; }
        string CriticalTime { get; }
    }
}