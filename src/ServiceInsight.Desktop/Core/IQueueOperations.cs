namespace Particular.ServiceInsight.Desktop.Core
{
    using System.Collections.Generic;
    using Models;

    public interface IQueueOperations
    {
        IList<Queue> GetQueues(string machineName);
        IList<MessageInfo> GetMessages(Queue queue);
        Queue CreateQueue(Queue queue, bool transactional);
        void DeleteQueue(Queue queueName);
        void Send(Queue queueName, object message);
        void DeleteMessage(Queue queue, string messageId);
        void PurgeQueue(Queue queue);
        void MoveMessage(Queue source, Queue destination, string messageId);
        int GetMessageCount(Queue queue);
        MessageBody GetMessageBody(Queue queue, string messageId);
        bool IsMsmqInstalled(string machineName);
    }
}