using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public interface IGatewayHeaderViewModel : IScreen
    {
        string From { get; }
        string To { get; }
        string DestinationSites { get; }
        string OriginatingSite { get; }
        string RouteTo { get; }
    }
}