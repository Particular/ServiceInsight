using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core
{
    public class AsyncQueueManager : QueueManager, IQueueManagerAsync
    {
        private readonly IQueueOperationsAsync _queueOperations;

        public AsyncQueueManager(IQueueOperationsAsync queueOperations) : base(queueOperations)
        {
            _queueOperations = queueOperations;
        }

        Task<IList<MessageInfo>> IQueueManagerAsync.GetMessages(Queue queue)
        {
            return _queueOperations.GetMessagesAsync(queue);
        }
    }

    public class QueueManager : IQueueManager
    {
        private readonly IQueueOperations _queueOperations;

        public QueueManager() : this(new MSMQueueOperations())
        {
        }

        public QueueManager(IQueueOperations queueOperations)
        {
            _queueOperations = queueOperations;
        }

        public virtual IList<Queue> GetQueues(string machineName = null)
        {
            if (machineName == null)
                machineName = Environment.MachineName;

            return _queueOperations.GetQueues(machineName);
        }

        public virtual IList<MessageInfo> GetMessages(Queue queue)
        {
            Guard.NotNull(() => queue, queue);
            
            return _queueOperations.GetMessages(queue);
        }

        public virtual MessageBody GetMessageBody(Queue queue, string messageId)
        {
            Guard.NotNull(() => queue, queue);

            return _queueOperations.GetMessageBody(queue, messageId);
        }

        public virtual int GetMessageCount(Queue queue)
        {
            Guard.NotNull(() => queue, queue);

            return _queueOperations.GetMessageCount(queue);
        }

        public virtual Queue CreatePrivateQueue(Queue queue, bool transactional = true)
        {
            Guard.NotNull(() => queue, queue);

            return _queueOperations.CreateQueue(queue, transactional);
        }

        public virtual void SendMessage(Queue queue, object msg)
        {
            Guard.NotNull(() => msg, msg);
            Guard.NotNull(() => queue, queue);

            _queueOperations.Send(queue, msg);
        }

        public virtual void DeleteQueue(Queue queue)
        {
            Guard.NotNull(() => queue, queue);
            Guard.NotNull(() => queue.Address, queue.Address);

            _queueOperations.DeleteQueue(queue);
        }

        public void MoveMessage(Queue source, Queue destination, string messageId)
        {
            Guard.NotNull(() => messageId, messageId);
            Guard.NotNull(() => destination, destination);

            _queueOperations.MoveMessage(source, destination, messageId);
        }

        public virtual void DeleteMessage(Queue queue, MessageInfo message)
        {
            Guard.NotNull(() => message, message);
            Guard.NotNull(() => queue, queue);

            _queueOperations.DeleteMessage(queue, message.Id);
        }

//        public void ConnectTo(string machineName)
//        {
//            Guard.NotNull(() => machineName, machineName);
//
//            //TODO: check ipv6 as well?
//            var ipv4 = GetIpAddressOrMachineName(machineName);
//
//            Guard.NotNullOrEmpty(() => ipv4, ipv4);
//            _machineName = machineName;
//        }

        public void Purge(Queue queue)
        {
            Guard.NotNull(() => queue, queue);
            _queueOperations.PurgeQueue(queue);
        }
    }
}