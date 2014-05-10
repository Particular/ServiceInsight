namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using System;

    public interface IPerformanceHeaderViewModel : IHeaderInfoViewModel, IPropertyDataProvider
    {
        DateTime? TimeSent { get; }
        DateTime? ProcessingStarted { get; }
        DateTime? ProcessingEnded { get; }
        string ProcessingTime { get; }
        string DeliveryTime { get; }
        string CriticalTime { get; }
    }
}