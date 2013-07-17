using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;

namespace NServiceBus.Profiler.Tests.Helpers
{
    public class FakeQueueManager : IQueueManagerAsync
    {
        public Dictionary<Queue, List<MessageInfo>> MessageQueue;

        public FakeQueueManager(Dictionary<Queue, List<MessageInfo>> messageQueue)
        {
            MessageQueue = messageQueue;
        }

        public IList<Queue> GetQueues(string machineName = null)
        {
            return MessageQueue.Keys.Select(x => x).ToList();
        }

        Task<int> IQueueManagerAsync.GetMessageCount(Queue queue)
        {
            throw new NotImplementedException();
        }

        Task<bool> IQueueManagerAsync.IsMsmqInstalled(string machineName)
        {
            throw new NotImplementedException();
        }

        bool IQueueManager.IsMsmqInstalled(string machineName)
        {
            return true;
        }

        public Task<IList<Queue>> GetQueues()
        {
            throw new NotImplementedException();
        }

        Task<IList<MessageInfo>> IQueueManagerAsync.GetMessages(Queue queue)
        {
            var t = new Task<IList<MessageInfo>>(() => MessageQueue[queue].ToArray());
            t.Start();
            return t;
        }

        Task<IList<Queue>> IQueueManagerAsync.GetQueues(string machineName)
        {
            throw new NotImplementedException();
        }

        public MessageBody GetMessageBody(Queue queue, string messageId)
        {
            throw new System.NotImplementedException();
        }

        Queue IQueueManager.CreatePrivateQueue(Queue queue, bool transactional)
        {
            throw new System.NotImplementedException();
        }

        IList<MessageInfo> IQueueManager.GetMessages(Queue queue)
        {
            return MessageQueue[queue].ToArray();
        }

        public Queue CreatePublicQueue(Address address, bool transactional)
        {
            throw new System.NotImplementedException();
        }

        public Queue CreatePrivateQueue(Address address, bool transactional)
        {
            var q = new Queue(address) {IsTransactional = transactional};
            MessageQueue.Add(q, new List<MessageInfo>());
            return q;
        }

        public void SendMessage(Queue destination, object message)
        {
            MessageQueue[destination].Add(new MessageInfo { Id = Guid.NewGuid().ToString(), Label = string.Empty, TimeSent = DateTime.Now });
        }

        public void DeleteQueue(Queue queue)
        {
            MessageQueue.Remove(queue);
        }

        public void MoveMessage(Queue source, Queue destination, string messageId)
        {
            var msg = MessageQueue[source].First(x => x.Id == messageId);
            MessageQueue[destination].Add(msg);
        }

        public void DeleteMessage(Queue queue, MessageInfo message)
        {
            //TODO: Fix message removal in memory
            //MessageQueue[queue].).Remove( message);
        }

        public void ConnectTo(string machineName)
        {
        }

        public void Purge(Queue queue)
        {
            MessageQueue[queue].Clear();
        }

        public int GetMessageCount(Queue queue)
        {
            throw new System.NotImplementedException();
        }
    }
}