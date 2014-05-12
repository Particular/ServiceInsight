namespace Particular.ServiceInsight.Desktop.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Messaging;
    using System.Threading.Tasks;
    using System.Transactions;
    using ExtensionMethods;
    using log4net;
    using Models;

    public class MSMQueueOperations : IQueueOperationsAsync
    {
        IMapper mapper;
        ILog logger = LogManager.GetLogger(typeof(IQueueOperations));

        public MSMQueueOperations(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public Task<IList<MessageInfo>> GetMessagesAsync(Queue queue)
        {
            return Task.Run(() => GetMessages(queue));
        }

        public Task<int> GetMessageCountAsync(Queue queue)
        {
            return Task.Run(() => GetMessageCount(queue));
        }

        public Task<IList<Queue>> GetQueuesAsync(string machineName)
        {
            return Task.Run(() => GetQueues(machineName));
        }

        public Task<bool> IsMsmqInstalledAsync(string machineName)
        {
            return Task.Run(() => IsMsmqInstalled(machineName));
        }

        public Task<Queue> CreateQueueAsync(Queue queue, bool isTransactional)
        {
            return Task.Run(() => CreateQueue(queue, isTransactional));
        }

        public IList<Queue> GetQueues(string machineName)
        {
            try
            {
                var queues = MessageQueue.GetPrivateQueuesByMachine(machineName).ToList();
                var mapped = queues.Select(x => mapper.MapQueue(x)).ToList();

                queues.ForEach(x => x.Close());

                return mapped;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not retreive queues on machine {0}.", machineName), ex);
                throw;
            }
        }

        public IList<MessageInfo> GetMessages(Queue queue)
        {
            using (var q = queue.AsMessageQueue())
            {
                q.MessageReadPropertyFilter.ClearAll();
                q.MessageReadPropertyFilter.Id = true;
                q.MessageReadPropertyFilter.Label = true;
                q.MessageReadPropertyFilter.SentTime = true;

                var result = new List<MessageInfo>();
                if (!queue.CanRead)
                {
                    return result;
                }

                var msgLoop = q.GetMessageEnumerator2();
                try
                {
                    while (msgLoop.MoveNext())
                    {
                        try
                        {
                            var currentMessage = msgLoop.Current;
                            if (currentMessage != null)
                            {
                                result.Add(mapper.MapInfo(currentMessage));
                            }
                        }
                        catch (MessageQueueException ex)
                        {
                            logger.Error("There was an error reading message from the queue.", ex);
                            throw;
                        }
                    }
                }
                finally
                {
                    msgLoop.Close();
                }

                return result;
            }
        }

        public MessageBody GetMessageBody(Queue queue, string messageId)
        {
            try
            {
                using (var q = queue.AsMessageQueue(QueueAccessMode.SendAndReceive))
                {
                    q.MessageReadPropertyFilter.SetAll();
                    q.MessageReadPropertyFilter.SourceMachine = true;

                    return mapper.MapBody(q.PeekById(messageId));
                }
            }
            catch (InvalidOperationException) //message is removed from the queue (by another process)
            {
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not read message {0} body.", messageId), ex);
                throw;
            }
        }

        public bool IsMsmqInstalled(string machineName)
        {
            try
            {
                MessageQueue.GetPrivateQueuesByMachine(machineName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Queue CreateQueue(Queue queue, bool transactional)
        {
            var path = queue.Address.ToShortFormatName();
            if(!QueueExists(queue)) //MessageQueue.Exist method does not accept format name
            {
                var q = MessageQueue.Create(path, transactional);
                return mapper.MapQueue(q);
            }
            
            return mapper.MapQueue(queue.AsMessageQueue());
        }

        public void DeleteQueue(Queue queue)
        {
            try
            {
                var format = queue.Address.ToFormatName();
                MessageQueue.Delete(format);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not delete queue {0}", queue), ex);
                throw;
            }
        }

        public void Send(Queue queue, object message)
        {
            try
            {
                using(var q = queue.AsMessageQueue(QueueAccessMode.SendAndReceive))
                {
                    q.Send(message, MessageQueueTransactionType.Single);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not send a message to queue {0}", queue), ex);
                throw;                
            }
        }

        public void DeleteMessage(Queue queue, string messageId)
        {
            try
            {
                using(var q = queue.AsMessageQueue(QueueAccessMode.SendAndReceive))
                {
                    q.ReceiveById(messageId);
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not delete message {0} to queue {1}", messageId, queue), ex);
                throw;                                
            }
        }

        public void PurgeQueue(Queue queue)
        {
            try
            {
                using(var q = queue.AsMessageQueue(QueueAccessMode.ReceiveAndAdmin))
                {
                    q.Purge();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not purge queue {0}", queue), ex);
                throw;                
            }
        }

        public void MoveMessage(Queue source, Queue destination, string messageId)
        {

            try
            {
                using(var tx = new TransactionScope())
                using(var queueDestination = destination.AsMessageQueue(QueueAccessMode.SendAndReceive))
                using(var queueSource = source.AsMessageQueue(QueueAccessMode.SendAndReceive))
                {
                    Guard.True(queueSource.CanRead, () => new QueueManagerException(string.Format("Can not read messages from queue {0}", source.Address)));
                    Guard.True(queueSource.Transactional, () => new QueueManagerException(string.Format("Queue {0} is not transactional", source.Address)));
                
                    Guard.True(queueDestination.CanRead, () => new QueueManagerException(string.Format("Can not read messages from queue {0}", destination.Address)));
                    Guard.True(queueDestination.Transactional, () => new QueueManagerException(string.Format("Queue {0} is not transactional", destination.Address)));

                    queueSource.MessageReadPropertyFilter.SetAll();
                    var msg = queueSource.ReceiveById(messageId, MessageQueueTransactionType.Automatic);

                    Guard.NotNull(() => msg, msg, () => new QueueManagerException("Message could not be loaded."));

                    queueDestination.Send(msg, MessageQueueTransactionType.Automatic);

                    tx.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not send a message {0} from queue {1} to {2}", messageId, source, destination), ex);
                throw;
            }
        }

        public int GetMessageCount(Queue queue)
        {
            var messageCount = 0;

            try
            {
                using (var q = queue.AsMessageQueue())
                {
                    q.MessageReadPropertyFilter.ClearAll();
                    var msgLoop = q.GetMessageEnumerator2();

                    if (queue.CanRead)
                    {
                        while (msgLoop.MoveNext())
                        {
                            messageCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Could not retrieve message count from queue {0}", queue), ex);
                throw;                
            }
            return messageCount;
        }

        bool QueueExists(Queue queue)
        {
            var allQueues = GetQueues(queue.Address.Machine);
            return allQueues.Any(q => q.Address.Queue.Contains(queue.Address.Queue, StringComparison.InvariantCultureIgnoreCase));
        }
    }   
}