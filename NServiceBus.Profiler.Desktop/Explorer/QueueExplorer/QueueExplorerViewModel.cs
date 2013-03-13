using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using System.Linq;
using NServiceBus.Profiler.Desktop.ScreenManager;
using DevExpress.Xpf.Editors.Helpers;

namespace NServiceBus.Profiler.Desktop.Explorer.QueueExplorer
{
    [View(typeof(IExplorerView))]
    public class QueueExplorerViewModel : Screen, IQueueExplorerViewModel
    {
        private readonly IQueueManager _queueManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManagerEx _windowManager;
        private bool _isFirstActivation = true;
        private IExplorerView _view;

        public QueueExplorerViewModel(
            IQueueManager queueManager,
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager)
        {
            _queueManager = queueManager;
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            Items = new BindableCollection<ExplorerItem>();
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
            if (_isFirstActivation)
            {
                _view.ExpandNode(MachineRoot);
                _isFirstActivation = false;
            }
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = view as IExplorerView;
            if (!IsConnected)
            {
                ConnectToQueue(Environment.MachineName);
            }
        }

        public virtual void DeleteSelectedQueue()
        {
            if (SelectedQueue == null)
                return;

            var selectedItem = SelectedNode;
            var confirmation = string.Format("The queue named {0} with all its messages and its subqueues will be removed. Continue?", SelectedQueue.Address);
            var result = _windowManager.ShowMessageBox(confirmation, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result != MessageBoxResult.OK)
                return;

            var itemsToRemove = new List<ExplorerItem>();
            foreach (var subqueue in selectedItem.Children.OfType<QueueExplorerItem>())
            {
                _queueManager.DeleteQueue(subqueue.Queue); 
                itemsToRemove.Add(subqueue);
            }

            foreach (var toRemove in itemsToRemove)
            {
                selectedItem.Children.Remove(toRemove);
            }

            _queueManager.DeleteQueue(SelectedQueue);
            MachineRoot.Children.Remove(selectedItem);
        }

        private void AddServerNode()
        {
            if (MachineRoot == null)
            {
                Items.Add(new ServerExplorerItem(ConnectedToAddress));
            }
        }

        public virtual void PartialRefresh()
        {
            if (MachineRoot == null)
                return;

            MachineRoot.Children.OfType<QueueExplorerItem>().ForEach(q =>
            {
                var count = _queueManager.GetMessageCount(q.Queue);
                q.UpdateMessageCount(count);
            });
        }

        public virtual ExplorerItem FolderRoot
        {
            get { return Items.FirstOrDefault(x => x is FolderExplorerItem); }
        }

        public virtual ExplorerItem MachineRoot
        {
            get { return Items.FirstOrDefault(x => x is ServerExplorerItem); }
        }

        public virtual ExplorerItem SelectedNode { get; set; }

        public virtual void ConnectToQueue(string computerName)
        {
            Guard.NotNull(() => computerName, computerName);

            //NOTE: check ipv6 as well?
            var ipv4 = Address.GetIpAddressOrMachineName(computerName);
            Guard.NotNullOrEmpty(() => ipv4, ipv4);

            ConnectedToAddress = ipv4;
            AddServerNode();
            FullRefresh();
        }

        public void FullRefresh()
        {
            if (MachineRoot == null)
                return;

            MachineRoot.Children.Clear();

            var queues = _queueManager.GetQueues(ConnectedToAddress).OrderBy(x => x.Address).ToList();

            SetupQueueNodes(queues);
            PartialRefresh();
        }

        public virtual string ConnectedToAddress { get; private set; }

        public int SelectedRowHandle { get; set; }

        public virtual Queue SelectedQueue
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

        public virtual void OnSelectedNodeChanged()
        {
            _eventAggregator.Publish(new SelectedQueueChanged(SelectedQueue));
        }

        public virtual IObservableCollection<ExplorerItem> Items { get; private set; }

        public virtual void ExpandNodes()
        {
            if (_view != null)
            {
                _view.Expand();
            }
        }

        private void SetupQueueNodes(IEnumerable<Queue> queues)
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

        private string GetMatchesSystemQueuesName(Queue queue)
        {
            return SubQueueNames.FirstOrDefault(q =>
            {
                return queue.Address.Queue.EndsWith(q, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        private bool IsConnected
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

        public void Handle(AutoRefreshBeat message)
        {
            PartialRefresh();
        }
    }
}