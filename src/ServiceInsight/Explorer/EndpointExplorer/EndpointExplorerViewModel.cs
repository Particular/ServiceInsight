namespace ServiceInsight.Explorer.EndpointExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using ExtensionMethods;
    using Framework;
    using Framework.Events;
    using Framework.Rx;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Pirac;
    using Startup;

    public class EndpointExplorerViewModel : RxScreen
    {
        static JsonSerializer serializer;

        static EndpointExplorerViewModel()
        {
            serializer = new JsonSerializer { ContractResolver = new SnakeCasePropertyNamesContractResolver() };
        }

        IRxEventAggregator eventAggregator;
        string initialEndpoint;

        public EndpointExplorerViewModel(IRxServiceControl serviceControl, IRxEventAggregator eventAggregator, CommandLineArgParser commandLineParser)
        {
            this.eventAggregator = eventAggregator;
            Items = new ObservableCollection<ExplorerItem>();

            initialEndpoint = commandLineParser.ParsedOptions.EndpointName;

            serviceControl.Endpoints().ObserveOnPiracMain().Subscribe(MergeEndpoints);

            eventAggregator.GetEvent<RequestSelectingEndpoint>().Subscribe(Handle);
            eventAggregator.GetEvent<SelectedExplorerItemChanged>().Subscribe(Handle);
        }

        private void MergeEndpoints(ServiceControlData e)
        {
            var root = Items.OfType<ServiceControlExplorerItem>().SingleOrDefault(i => i.Name == e.Url);

            if (root == null)
            {
                if (!Items.Any())
                {
                    root = new ServiceControlExplorerItem(e.Url);
                    root.IsExpanded = true;

                    Items.Add(root);

                    SelectedNode = root;
                }
                else
                {
                    return;
                }
            }

            var toRemove = root.Children.ToList();
            var endpointInstancesGroupedByName = e.Data.OrderBy(x => x.name).GroupBy(x => x.name);
            foreach (var endpointGroup in endpointInstancesGroupedByName)
            {
                var instances = endpointGroup.ToList();
                var endpoint = instances.Cast<JObject>().First().ToObject<Models.Endpoint>(serializer);
                var node = root.GetEndpointNode(endpoint);
                if (node != null)
                {
                    toRemove.Remove(node);
                }
                else
                {
                    var hostNames = instances.Select(instance => instance.host_display_name).Distinct();
                    var tooltip = string.Join(", ", hostNames);

                    node = new AuditEndpointExplorerItem(endpoint, tooltip);
                    root.Children.Add(node);
                }

                if (!initialEndpoint.IsEmpty() && string.Equals(node.Name, initialEndpoint, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedNode = node;
                    initialEndpoint = "";
                }
            }
            if (toRemove.Any())
            {
                var saveSelectedNode = SelectedNode;
                root.Children.RemoveRange(toRemove);
                SelectedNode = saveSelectedNode;
            }
        }

        public ICollection<ExplorerItem> Items { get; }

        public ExplorerItem SelectedNode { get; set; }

        public void OnSelectedNodeChanged()
        {
            eventAggregator.Publish(new SelectedExplorerItemChanged(SelectedNode));
        }

        void Handle(RequestSelectingEndpoint message)
        {
            foreach (var item in Items.OfType<ServiceControlExplorerItem>())
            {
                if (item.EndpointExists(message.Endpoint))
                {
                    var node = item.GetEndpointNode(message.Endpoint);
                    SelectedNode = node;
                    break;
                }
            }
        }

        void Handle(SelectedExplorerItemChanged message)
        {
            var endpoint = message.SelectedExplorerItem as AuditEndpointExplorerItem;
            if (endpoint != null)
            {
                Handle(new RequestSelectingEndpoint(endpoint.Endpoint));
            }
        }
    }
}