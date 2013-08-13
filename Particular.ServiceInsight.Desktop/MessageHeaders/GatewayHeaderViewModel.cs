using System;
using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.PresentationFramework.ApplicationModel;
using Particular.ServiceInsight.Desktop.Core;
using Particular.ServiceInsight.Desktop.Core.MessageDecoders;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    [TypeConverter(typeof(HeaderInfoTypeConverter))]
    public class GatewayHeaderViewModel : HeaderInfoViewModelBase, IGatewayHeaderViewModel
    {
        public GatewayHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager) 
            : base(eventAggregator, decoder, queueManager)
        {
            DisplayName = "Gateway";
        }

        [Description("From address")]
        public string From { get; set; }

        [Description("To address")]
        public string To { get; set; }

        [Description("Destination sites")]
        public string DestinationSites { get; set; }

        [Description("Originating site")]
        public string OriginatingSite { get; set; }

        [Description("Route to address")]
        public string RouteTo { get; set; }

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