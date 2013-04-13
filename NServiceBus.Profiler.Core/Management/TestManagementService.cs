using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;
using System.Linq;

namespace NServiceBus.Profiler.Core.Management
{
    public class TestManagementService : IManagementService
    {
        private PagedResult<StoredMessage> audit;
        private List<StoredMessage> conversation;

        public TestManagementService()
        {
            CreateMessageList();
        }

        private string CreateConversation()
        {
            var root = Guid.NewGuid().ToString();
            var firstChild = Guid.NewGuid().ToString();
            var secondChild = Guid.NewGuid().ToString();

            conversation = new List<StoredMessage>(new[]
            {
                CreateConversationMessage(root, null, "Orders.OrderCreated, Messages", fromEndpoint: "Hadi-PC", toEndpoint: "Remote"),
                CreateConversationMessage(Guid.NewGuid().ToString(), root, "Orders.NotifyWarehouse, Messages", deferred: true, fromEndpoint: "Hadi-PC", toEndpoint: "Remote"),
                CreateConversationMessage(firstChild, root, "Orders.OrderProcessed, Messages", fromEndpoint: "Hadi-PC", toEndpoint: "Remote"), 
                CreateConversationMessage(secondChild, firstChild, "Notifications.SendEmail, Messages", fromEndpoint: "Hadi-PC", toEndpoint: "Remote"), 
                CreateConversationMessage(Guid.NewGuid().ToString(), secondChild, "Notification.SendMail, Messages", status: MessageStatus.Failed, fromEndpoint: "Hadi-PC", toEndpoint: "Remote"),
                CreateConversationMessage(Guid.NewGuid().ToString(), secondChild, "Notification.SendMail, Messages", status: MessageStatus.RepeatedFailures, fromEndpoint: "Hadi-PC", toEndpoint: "Remote")
            });

            return root;
        }

        private void CreateMessageList()
        {
            var conversationId = CreateConversation();

            audit = new PagedResult<StoredMessage>
            {
                TotalCount = 1,
                Result = new List<StoredMessage>
                {
                    new StoredMessage
                    {
                        Id = Guid.NewGuid().ToString(),
                        Status = MessageStatus.Successfull,
                        ConversationId = conversationId,
                        MessageType = "Orders.OrderCreated, Messages",
                        TimeSent = DateTime.Now.AddMinutes(-5),
                    }
                }
            };
        }

        public Task<PagedResult<StoredMessage>> GetErrorMessages(string serviceUrl)
        {
            return Task.Run(() => new PagedResult<StoredMessage>
            {
                Result = new List<StoredMessage>(new[]
                {
                    new StoredMessage(),
                    new StoredMessage()
                }),
                TotalCount = 2
            });
        }

        public Task<PagedResult<StoredMessage>> GetAuditMessages(string serviceUrl, Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool @ascending = false)
        {
            return Task.Run(() => audit);
        }

        public Task<List<StoredMessage>> GetConversationById(string serviceUrl, string conversationId)
        {
            return Task.Run(() =>
            {
                return conversation.ToList();
            });
        }

        public Task<List<Endpoint>> GetEndpoints(string serviceUrl)
        {
            return Task.Run(() => new List<Endpoint>()
            {
                new Endpoint
                {
                    Url = "http://remote",
                    Machine = "Remote-PC",
                    Name = "Remote"
                },
                new Endpoint
                {
                    Url = "http://localhost",
                    Machine = "Hadi-PC",
                    Name = "Hadi-PC"
                }
            });
        }

        public Task<bool> IsAlive(string serviceUrl)
        {
            return Task.Run(() => true);
        }

        private StoredMessage CreateConversationMessage(string id, string parentId, string messageType, bool deferred = false, MessageStatus status = MessageStatus.Successfull, string fromEndpoint = null, string toEndpoint = null)
        {
            return new StoredMessage
            {
                Id = id,
                Body = "{\"Messages\":{\"@xmlns:xsi\":\"http://www.w3.org/2001/XMLSchema-instance\",\"@xmlns:xsd\":\"http://www.w3.org/2001/XMLSchema\",\"@xmlns\":\"http://tempuri.net/MyMessages\",\"IMyEvent\":{\"EventId\":\"22b4ee64-8691-4f53-8baf-1769c3c7b49d\",\"Time\":\"2013-02-06T21:23:37.6962968+11:00\",\"Duration\":\"P1DT3H46M39S\"}}}",
                RelatedToMessageId = parentId,
                Status = status,
                IsDeferredMessage = deferred,
                TimeSent = DateTime.Now,
                MessageType = messageType,
                OriginatingEndpoint = new Endpoint {Name = fromEndpoint},
                ReceivingEndpoint = new Endpoint {Name = toEndpoint}
            };
        }

        private byte[] CreateStandardHeaders()
        {
            var headers = new List<HeaderInfo>();
            headers.Add(new HeaderInfo { Key = "Version", Value = "4.0.0.0" });

            return null;
        }

        public class SampleBusinessMessage
        {
        }
    }
}