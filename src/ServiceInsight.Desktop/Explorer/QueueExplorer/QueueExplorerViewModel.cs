namespace Particular.ServiceInsight.Desktop.Explorer.QueueExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.PresentationFramework.Views;
    using Core;
    using Core.UI.ScreenManager;
    using Events;
    using ExtensionMethods;
    using Models;
    using Shell;

    [View(typeof(QueueExplorerView))]
    public class QueueExplorerViewModel : Screen, IQueueExplorerViewModel
    {
        IQueueManagerAsync queueManager;
        IEventAggregator eventAggregator;
        IWindowManagerEx windowManager;
        INetworkOperations networkOperations;
        bool isFirstActivation = true;
        IExplorerView view;

        public QueueExplorerViewModel(
            IQueueManagerAsync queueManager,
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager,
            INetworkOperations networkOperations)
        {
            this.queueManager = queueManager;
            this.eventAggregator = eventAggregator;
            this.windowManager = windowManager;
            this.networkOperations = networkOperations;
            Items = new BindableCollection<ExplorerItem>();
            IsMSMQInstalled = true;
        }

        public static IList<string> SubQueueNames = new[]
        {
            ".subscriptions",
            ".timeouts",
            ".errors",
            ".retries",
            ".worker",
            ".distributor.storage",
            ".gateway",
            ".notifications",
            ".timeoutsdispatcher",
            ".distributor.control"
        };

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (isFirstActivation)
            {
                this.view.ExpandNode(MachineRoot);
                isFirstActivation = false;
            }
        }

        public override async void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            this.view = view as IExplorerView;
            if (!IsConnected)
            {
                IsMSMQInstalled = await queueManager.IsMsmqInstalled(LocalMachineName);
                if (IsMSMQInstalled)
                {
                    await ConnectToQueue(LocalMachineName);
                }
            }
        }

        public void DeleteSelectedQueue()
        {
            if (SelectedQueue == null)
                return;

            var selectedItem = SelectedNode;
            var confirmation = string.Format("The queue named {0} with all its messages and its subqueues will be removed. Continue?", SelectedQueue.Address);
            var dialogTitle = string.Format("Delete Queue: {0}", selectedItem.Name);
            var result = windowManager.ShowMessageBox(confirmation, dialogTitle, MessageBoxButton.OKCancel, MessageBoxImage.Question, defaultChoice: MessageChoice.Cancel);
            
            if (result != MessageBoxResult.OK)
                return;

            var itemsToRemove = new List<ExplorerItem>();
            foreach (var subqueue in selectedItem.Children.OfType<QueueExplorerItem>())
            {
                queueManager.DeleteQueue(subqueue.Queue); 
                itemsToRemove.Add(subqueue);
            }

            foreach (var toRemove in itemsToRemove)
            {
                selectedItem.Children.Remove(toRemove);
            }

            queueManager.DeleteQueue(SelectedQueue);
            MachineRoot.Children.Remove(selectedItem);
        }

        public new IShellViewModel Parent
        {
            get { return (IShellViewModel)base.Parent; }
        }

        void AddServerNode()
        {
            if (MachineRoot == null)
            {
                Items.Add(new QueueServerExplorerItem(ConnectedToAddress));
            }
        }

        public void Navigate(string navigateUri)
        {
            networkOperations.Browse(navigateUri);
        }

        public string LocalMachineName
        {
            get { return Environment.MachineName; }
        }

        public bool IsMSMQInstalled { get; private set; }

        public ExplorerItem FolderRoot
        {
            get { return Items.FirstOrDefault(x => x is FolderExplorerItem); }
        }

        public ExplorerItem MachineRoot
        {
            get { return Items.FirstOrDefault(x => x is QueueServerExplorerItem); }
        }

        public ExplorerItem SelectedNode { get; set; }

        public async Task ConnectToQueue(string computerName)
        {
            Guard.NotNull(() => computerName, computerName);

            //NOTE: check ipv6 as well?
            var ipv4 = Address.GetIpAddressOrMachineName(computerName);
            Guard.NotNullOrEmpty(() => ipv4, ipv4);

            ConnectedToAddress = ipv4;
            AddServerNode();
            await RefreshData();
        }

        public async Task RefreshData()
        {
            if (MachineRoot == null)
                return;

            MachineRoot.Children.Clear();

            var queues = await queueManager.GetQueues(ConnectedToAddress);
            var sortedQueues = queues.OrderBy(x => x.Address).ToList();

            SetupQueueNodes(sortedQueues);

            foreach (var explorerItem in MachineRoot.Children.OfType<QueueExplorerItem>().ToList())
            {
                var messageCount = await queueManager.GetMessageCount(explorerItem.Queue);
                explorerItem.UpdateMessageCount(messageCount);
            }
        }

        public string ConnectedToAddress { get; private set; }

        public Queue SelectedQueue
        {
            get
            {
                var selectedQueue = SelectedNode as QueueExplorerItem;
                if (selectedQueue != null)
                {
                    return ((QueueExplorerItem)SelectedNode).Queue;
                }

                return null;
            }
        }

        public void OnSelectedNodeChanged()
        {
            eventAggregator.Publish(new SelectedExplorerItemChanged(SelectedNode));
        }

        public IObservableCollection<ExplorerItem> Items { get; private set; }

        public void ExpandNodes()
        {
            if (view != null)
            {
                view.Expand();
            }
        }

        void SetupQueueNodes(IEnumerable<Queue> queues)
        {
            if (MachineRoot == null)
                return;

            foreach (var queue in queues)
            {
                if (queue == null)
                    continue;

                QueueExplorerItem queueToAdd = null;
                var parentTreeNode = MachineRoot;
                var systemQueue = GetMatchesSystemQueuesName(queue);

                if (systemQueue != null)
                {
                    var parentName = queue.Address.Queue.Replace(systemQueue, string.Empty);
                    var parentNode = MachineRoot.Children.FirstOrDefault(q => q.Name != null && q.Name == parentName);

                    if (parentNode != null)
                    {
                        parentTreeNode = parentNode;
                        queueToAdd = new QueueExplorerItem(queue, systemQueue.Remove(0, 1));
                    }
                }

                if (queueToAdd == null)
                {
                    queueToAdd = new QueueExplorerItem(queue);
                }

                parentTreeNode.Children.Add(queueToAdd);
            }
        }

        string GetMatchesSystemQueuesName(Queue queue)
        {
            return SubQueueNames.FirstOrDefault(q => queue.Address.Queue.EndsWith(q, StringComparison.InvariantCultureIgnoreCase));
        }

        bool IsConnected
        {
            get { return !ConnectedToAddress.IsEmpty(); }
        }

        public void Handle(QueueMessageCountChanged message)
        {
            var node = SelectedNode as QueueExplorerItem;
            if (node != null && node.Queue == message.Queue)
            {
                node.UpdateMessageCount(message.Count);
            }
        }
    }
}