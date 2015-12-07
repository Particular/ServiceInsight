namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Caliburn.Micro;
    using Models;
    using Particular.ServiceInsight.Desktop.Framework.MessageDecoders;
    using Particular.ServiceInsight.Desktop.MessageList;

    public class GatewayHeaderViewModel : HeaderInfoViewModelBase, IPropertyDataProvider
    {
        public GatewayHeaderViewModel(
            IEventAggregator eventAggregator,
            IContentDecoder<IList<HeaderInfo>> decoder,
            MessageSelectionContext selectionContext)
            : base(eventAggregator, decoder, selectionContext)
        {
            DisplayName = "Gateway";
        }

        [Description("From address")]
        public string From { get; private set; }

        [Description("To address")]
        public string To { get; private set; }

        [Description("Destination sites")]
        public string DestinationSites { get; private set; }

        [Description("Originating site")]
        public string OriginatingSite { get; private set; }

        [Description("Route to address")]
        public string RouteTo { get; private set; }

        protected override void MapHeaderKeys()
        {
            ConditionsMap.Add(h => h.Key.EndsWith(".From", StringComparison.OrdinalIgnoreCase), h => From = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(".To", StringComparison.OrdinalIgnoreCase), h => To = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(".DestinationSites", StringComparison.OrdinalIgnoreCase), h => DestinationSites = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(".OriginatingSite", StringComparison.OrdinalIgnoreCase), h => OriginatingSite = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(".Header.RouteTo", StringComparison.OrdinalIgnoreCase), h => RouteTo = h.Value);
        }

        protected override void ClearHeaderValues()
        {
            From = null;
            To = null;
            DestinationSites = null;
            OriginatingSite = null;
            RouteTo = null;
        }
    }
}