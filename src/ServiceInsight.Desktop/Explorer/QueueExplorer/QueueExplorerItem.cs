namespace Particular.ServiceInsight.Desktop.Explorer.QueueExplorer
{
    using System.Drawing;
    using System.Globalization;
    using Models;
    using Properties;

    public class QueueExplorerItem : ExplorerItem
    {
        string displayName;
        Queue queue;
        string queueName;

        public QueueExplorerItem(Queue queue) : base(queue.Address.Queue)
        {
            this.queue = queue;
            queueName = queue.Address.Queue;
        }

        public QueueExplorerItem(Queue queue, string displayName) : base(queue.Address.Queue)
        {
            this.queue = queue;
            queueName = this.displayName = displayName;
        }

        public Queue Queue
        {
            get { return queue; }
        }

        public override Bitmap Image
        {
            get { return Resources.Queue; }
        }

        public override string DisplayName
        {
            get { return displayName; }
        }

        public void UpdateMessageCount(int count)
        {
            displayName = count > 0 ? string.Format("{0} ({1})", queueName, count.ToString(CultureInfo.InvariantCulture)) : queueName;
            NotifyOfPropertyChange(() => DisplayName);
        }
    }
}