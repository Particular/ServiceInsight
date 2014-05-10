namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    public interface IGatewayHeaderViewModel : IPropertyDataProvider
    {
        string From { get; }
        string To { get; }
        string DestinationSites { get; }
        string OriginatingSite { get; }
        string RouteTo { get; }
    }
}