using System.Collections.Generic;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core
{
    public interface IQueueOperations
    {
        /// <summary>
        /// Gets list of all queues (private and public)
        /// </summary>
        IList<Queue> GetQueues(string machineName);

        /// <summary>
        /// Gets a list of all messages in the queue
        /// </summary>
        IList<MessageInfo> GetMessages(Queue queue);

        /// <summary>
        /// Creates a new queue
        /// </summary>
        Queue CreateQueue(Queue queue, bool transactional);

        /// <summary>
        /// Removes a queue
        /// </summary>
        void DeleteQueue(Queue queueName);

        /// <summary>
        /// Sends a message to the queue
        /// </summary>
        void Send(Queue queueName, object message);

        /// <summary>
        /// Deletes the message from the queue
        /// </summary>
        void DeleteMessage(Queue queue, string messageId);

        /// <summary>
        /// Deletes all messages in the queue
        /// </summary>
        void PurgeQueue(Queue queue);

        /// <summary>
        /// Moves a message from the source queue to destination queue
        /// </summary>
        void MoveMessage(Queue source, Queue destination, string messageId);

        /// <summary>
        /// Gets the number of available message in the queue
        /// </summary>
        int GetMessageCount(Queue queue);

        /// <summary>
        /// Returns the message body/header for the given messageId
        /// </summary>
        MessageBody GetMessageBody(Queue queue, string messageId);
    }
}