namespace Particular.ServiceInsight.Desktop.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public class AsyncQueueManager : QueueManager, IQueueManagerAsync
    {
        private readonly IQueueOperationsAsync queueOperations;

        public AsyncQueueManager(IQueueOperationsAsync queueOperations) : base(queueOperations)
        {
            this.queueOperations = queueOperations;
        }

        Task<IList<MessageInfo>> IQueueManagerAsync.GetMessages(Queue queue)
        {
            return queueOperations.GetMessagesAsync(queue);
        }

        Task<IList<Queue>> IQueueManagerAsync.GetQueues(string machineName)
        {
            return queueOperations.GetQueuesAsync(machineName);
        }

        Task<bool> IQueueManagerAsync.IsMsmqInstalled(string machineName)
        {
            return queueOperations.IsMsmqInstalledAsync(machineName);
        }

        Task<IList<Queue>> IQueueManagerAsync.GetQueues()
        {
            return queueOperations.GetQueuesAsync(Environment.MachineName);
        }

        public Task<Queue> CreatePrivateQueueAsync(Queue queue, bool isTransactional = true)
        {
            Guard.NotNull(() => queue, queue);
            return queueOperations.CreateQueueAsync(queue, isTransactional);
        }

        Task<int> IQueueManagerAsync.GetMessageCount(Queue queue)
        {
            return queueOperations.GetMessageCountAsync(queue);
        }
    }

    public class QueueManager : IQueueManager
    {
        private readonly IQueueOperations queueOperations;

        public QueueManager() : this(new MSMQueueOperations(new DefaultMapper()))
        {
        }

        public QueueManager(IQueueOperations queueOperations)
        {
            this.queueOperations = queueOperations;
        }

        public IList<Queue> GetQueues(string machineName = null)
        {
            if (machineName == null)
                machineName = Environment.MachineName;

            return queueOperations.GetQueues(machineName);
        }

        public IList<MessageInfo> GetMessages(Queue queue)
        {
            Guard.NotNull(() => queue, queue);
            
            return queueOperations.GetMessages(queue);
        }

        public MessageBody GetMessageBody(Queue queue, string messageId)
        {
            Guard.NotNull(() => queue, queue);

            return queueOperations.GetMessageBody(queue, messageId);
        }

        public int GetMessageCount(Queue queue)
        {
            Guard.NotNull(() => queue, queue);

            return queueOperations.GetMessageCount(queue);
        }

        public bool IsMsmqInstalled(string machineName)
        {
            return queueOperations.IsMsmqInstalled(machineName);
        }

        public Queue CreatePrivateQueue(Queue queue, bool transactional = true)
        {
            Guard.NotNull(() => queue, queue);

            return queueOperations.CreateQueue(queue, transactional);
        }

        public void SendMessage(Queue queue, object msg)
        {
            Guard.NotNull(() => msg, msg);
            Guard.NotNull(() => queue, queue);

            queueOperations.Send(queue, msg);
        }

        public void DeleteQueue(Queue queue)
        {
            Guard.NotNull(() => queue, queue);
            Guard.NotNull(() => queue.Address, queue.Address);

            queueOperations.DeleteQueue(queue);
        }

        public void MoveMessage(Queue source, Queue destination, string messageId)
        {
            Guard.NotNull(() => messageId, messageId);
            Guard.NotNull(() => destination, destination);

            queueOperations.MoveMessage(source, destination, messageId);
        }

        public void DeleteMessage(Queue queue, MessageInfo message)
        {
            Guard.NotNull(() => message, message);
            Guard.NotNull(() => queue, queue);

            queueOperations.DeleteMessage(queue, message.Id);
        }

        public void Purge(Queue queue)
        {
            Guard.NotNull(() => queue, queue);
            queueOperations.PurgeQueue(queue);
        }
    }
}