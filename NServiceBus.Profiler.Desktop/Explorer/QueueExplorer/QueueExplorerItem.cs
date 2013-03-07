using System.Drawing;
using System.Globalization;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer.QueueExplorer
{
    public class QueueExplorerItem : ExplorerItem
    {
        private string _displayName;
        private readonly Queue _queue;
        private string _queueName;

        public QueueExplorerItem(Queue queue) : base(queue.Address.Queue)
        {
            _queue = queue;
            _queueName = queue.Address.Queue;
        }

        public QueueExplorerItem(Queue queue, string displayName) : base(queue.Address.Queue)
        {
            _queue = queue;
            _queueName = _displayName = displayName;
        }

        public Queue Queue
        {
            get { return _queue; }
        }

        public override Bitmap Image
        {
            get { return Resources.Queue; }
        }

        public override string DisplayName
        {
            get { return _displayName; }
        }

        public void UpdateMessageCount(int count)
        {
            _displayName = count > 0 ? string.Format("{0} ({1})", _queueName, count.ToString(CultureInfo.InvariantCulture)) : _queueName;
            NotifyOfPropertyChange(() => DisplayName);
        }
    }
}