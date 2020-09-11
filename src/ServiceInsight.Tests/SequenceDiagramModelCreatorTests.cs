namespace ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::ServiceInsight.SequenceDiagram;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Models;

    [TestFixture]
    class SequenceDiagramModelCreatorTests
    {
        IMessageCommandContainer container;

        [SetUp]
        public void Setup()
        {
            container = Substitute.For<IMessageCommandContainer>();
        }

        ModelCreator GetModelCreator(List<StoredMessage> messages) => new ModelCreator(messages, container);

        [Test]
        public void NoMessages()
        {
            var messages = new List<StoredMessage>();
            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void SameSenderAndReceiver()
        {
            var messages = new List<StoredMessage>
            {
                new StoredMessage
                {
                    MessageId = "1",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A",
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.Version,
                            Value = "1"
                        },
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "10"
                        }
                    }
                }
            };

            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("A", result[0].Name);
        }

        [Test]
        public void SameLogicalEndpointsWithDifferentVersions()
        {
            var testHost = Guid.NewGuid().ToString();
            
            var messages = new List<StoredMessage>
            {
                new StoredMessage
                {
                    MessageId = "1",
                    SendingEndpoint = new Endpoint()
                    {
                        Name = "A",
                        Host = nameof(testHost),
                        HostId = testHost
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "A",
                        Host = nameof(testHost),
                        HostId = testHost
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.Version,
                            Value = "1.0.0"
                        },
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "10"
                        }
                    }
                },
                new StoredMessage
                {
                    MessageId = "2",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A",
                        Host = nameof(testHost),
                        HostId = testHost
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "A",
                        Host = nameof(testHost),
                        HostId = testHost
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.Version,
                            Value = "2.0.0"
                        },
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "10"
                        }
                    }
                },
                new StoredMessage
                {
                    MessageId = "3",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A",
                        Host = nameof(testHost),
                        HostId = testHost
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "A",
                        Host = nameof(testHost),
                        HostId = testHost
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.Version,
                            Value = "3.0.0"
                        },
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "10"
                        }
                    }
                }
            };

            var creator = GetModelCreator(messages);
            var endpoints = creator.Endpoints;
            
            Assert.AreEqual(1, endpoints.Count);
            Assert.AreEqual("A", endpoints[0].Name);
            Assert.AreEqual(nameof(testHost), endpoints[0].Host);
            Assert.AreEqual(testHost, endpoints[0].HostId);
            Assert.AreEqual(1, endpoints[0].Hosts.Count);
            Assert.AreEqual(3, endpoints[0].Hosts[0].HostVersions.Count);
            Assert.AreEqual("1.0.0", endpoints[0].Hosts[0].HostVersions[0]);
            Assert.AreEqual("2.0.0", endpoints[0].Hosts[0].HostVersions[1]);
            Assert.AreEqual("3.0.0", endpoints[0].Hosts[0].HostVersions[2]);
        }

        [Test]
        public void SequentialFlowWithDistinctEndpoints()
        {
            var messages = new List<StoredMessage>
            {
                new StoredMessage
                {
                    MessageId = "3",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "D"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "2"
                        }
                    }
                },
                new StoredMessage
                {
                    MessageId = "1",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>()
                },
                new StoredMessage
                {
                    MessageId = "2",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    }
                }
            };

            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual("B", result[1].Name);
            Assert.AreEqual("C", result[2].Name);
            Assert.AreEqual("D", result[3].Name);
        }

        [Test]
        public void SequentialFlowWithMissingRelatedTo()
        {
            var messages = new List<StoredMessage>
            {
                new StoredMessage
                {
                    MessageId = "2",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    }
                }
            };

            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual("B", result[1].Name);
        }

        [Test]
        public void SequentialFlowWithSharedSourceEndpoints()
        {
            var messages = new List<StoredMessage>
            {
                new StoredMessage
                {
                    MessageId = "2",
                    ProcessedAt = DateTime.UtcNow.AddMinutes(1),
                    SendingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    }
                },
                new StoredMessage
                {
                    MessageId = "3",
                    ProcessedAt = DateTime.UtcNow.AddMinutes(2),
                    SendingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "D"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    }
                },
                new StoredMessage
                {
                    MessageId = "1",
                    ProcessedAt = DateTime.UtcNow,
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>()
                }
            };

            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual("B", result[1].Name);
            Assert.AreEqual("C", result[2].Name);
            Assert.AreEqual("D", result[3].Name);
        }

        [Test]
        public void SequentialFlowWithSharedDestinationEndpoints()
        {
            var messages = new List<StoredMessage>
            {
                new StoredMessage
                {
                    MessageId = "2",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    }
                },
                new StoredMessage
                {
                    MessageId = "3",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    }
                },
                new StoredMessage
                {
                    MessageId = "1",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>()
                }
            };

            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual("B", result[1].Name);
            Assert.AreEqual("C", result[2].Name);
        }

        [Test]
        public void OrderOfOutArrowsFromHandler()
        {
            var currentDateTime = DateTime.UtcNow;

            var messages = new List<StoredMessage>
            {
                new StoredMessage
                {
                    MessageId = "3",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "D"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    },
                    MessageType = "Message3",
                    ProcessedAt = currentDateTime.AddMinutes(2),
                    TimeSent = currentDateTime.AddSeconds(3),
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                },
                new StoredMessage
                {
                    MessageId = "2",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    },
                    MessageType = "Message2",
                    ProcessedAt = currentDateTime.AddMinutes(1),
                    TimeSent = currentDateTime.AddSeconds(2),
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                },
                new StoredMessage
                {
                    MessageId = "4",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    },
                    MessageType = "Message4",
                    ProcessedAt = currentDateTime.AddMinutes(1),
                    TimeSent = currentDateTime.AddSeconds(4),
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                }
            };

            var creator = GetModelCreator(messages);
            var result = creator.Endpoints.ToList();
            var outArrows = result[0].Handlers[0].Out.ToList();

            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual(1, result[0].Handlers.Count);
            Assert.AreEqual(3, outArrows.Count);
            Assert.AreEqual("2", outArrows[0].MessageId);
            Assert.AreEqual("3", outArrows[1].MessageId);
            Assert.AreEqual("4", outArrows[2].MessageId);
        }

        [Test]
        public void SequentialOrderOfHandlers()
        {
            var currentDateTime = DateTime.UtcNow;

            var messages = new List<StoredMessage>
            {
                new StoredMessage
                {
                    MessageId = "3",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "D"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "2"
                        }
                    },
                    MessageType = "Message3",
                    ProcessedAt = currentDateTime.AddMinutes(2),
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                },
                new StoredMessage
                {
                    MessageId = "1",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>(),
                    MessageType = "Message1",
                    ProcessedAt = currentDateTime,
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                },
                new StoredMessage
                {
                    MessageId = "2",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    },
                    MessageType = "Message2",
                    ProcessedAt = currentDateTime.AddMinutes(1),
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                },
                new StoredMessage
                {
                    MessageId = "4",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    },
                    MessageType = "Message4",
                    ProcessedAt = currentDateTime.AddMinutes(1),
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                },
                new StoredMessage
                {
                    MessageId = "5",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "C"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "4"
                        }
                    },
                    MessageType = "Message5",
                    ProcessedAt = currentDateTime.AddMinutes(2),
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                }
            };

            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(1, result[0].Handlers.Count);
            Assert.AreEqual(2, result[1].Handlers.Count);
            Assert.AreEqual(2, result[2].Handlers.Count);
            Assert.AreEqual(1, result[3].Handlers.Count);

            Assert.AreEqual("Message1", result[1].Handlers[0].Name);
            Assert.AreEqual("Message5", result[1].Handlers[1].Name);
        }

        [Test]
        public void HandlerMessageInAndOut()
        {
            var currentDateTime = DateTime.UtcNow;

            var messages = new List<StoredMessage>
            {
                new StoredMessage
                {
                    MessageId = "2",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    },
                    MessageType = "Message2",
                    ProcessedAt = currentDateTime,
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                },
                new StoredMessage
                {
                    MessageId = "3",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    },
                    MessageType = "Message3",
                    ProcessedAt = currentDateTime,
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                },
                new StoredMessage
                {
                    MessageId = "4",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    },
                    MessageType = "Message4",
                    ProcessedAt = currentDateTime,
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                },
                new StoredMessage
                {
                    MessageId = "5",
                    SendingEndpoint = new Endpoint
                    {
                        Name = "A"
                    },
                    ReceivingEndpoint = new Endpoint
                    {
                        Name = "B"
                    },
                    Headers = new List<StoredMessageHeader>
                    {
                        new StoredMessageHeader
                        {
                            Key = MessageHeaderKeys.RelatedTo,
                            Value = "1"
                        }
                    },
                    MessageType = "Message5",
                    ProcessedAt = currentDateTime,
                    MessageIntent = MessageIntent.Send,
                    Status = MessageStatus.Successful,
                }
            };

            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(1, result[0].Handlers.Count);
            Assert.AreEqual(4, result[1].Handlers.Count);
            Assert.AreEqual(4, result[0].Handlers[0].Out.Count());
        }

        [Test]
        public void HandlersAreInTheCorrectOrder_WhenStartOfConversationIsMissing()
        {
            // Based on customer data
            var start = DateTime.UtcNow;

            var messages = new List<StoredMessage>
            {
                // Msg 1 and 2 are not present
                Msg("3", "1", "ProductPurchaseTakingTooLong", start.AddSeconds(19).AddMilliseconds(305698), "Provisioning.CRM.Orchestrator", start.AddSeconds(19).AddMilliseconds(352572), "Provisioning.Communication.Orchestrator"),
                Msg("4", "2", "GpOrderSagaTimeout", start.AddSeconds(63).AddMilliseconds(647730), "Provisioning.GP.Orchestrator", start.AddMinutes(9).AddSeconds(24).AddMilliseconds(352189), "Provisioning.GP.Orchestrator"),
                Msg("5", "1", "CrmOrderSagaTimeout", start.AddMinutes(5).AddSeconds(19).AddMilliseconds(233001), "Provisioning.CRM.Orchestrator", start.AddMinutes(5).AddSeconds(19).AddMilliseconds(795543), "Provisioning.CRM.Orchestrator"),
            };

            var creator = GetModelCreator(messages);

            var result = creator.Handlers;

            Assert.AreEqual(5, result.Count, "There should by 5 handlers");
            Assert.AreEqual("1", result[0].ID, "Earliest handler should be for message 1 (even though we don't know what that was)");
            Assert.AreEqual("ProductPurchaseTakingTooLong", result[1].Name, "Second handler should be for ProductPurchaseTakingTooLong");
            Assert.AreEqual("2", result[2].ID, "Third handler is for message 2 (even though we don't know what that was)");
            Assert.AreEqual("GpOrderSagaTimeout", result[3].Name, "Fourth handler should be for the GpOrderSagaTimeout");
            Assert.AreEqual("CrmOrderSagaTimeout", result[4].Name, "Latest handler should be for CrmOrderSagaTimeout");
        }

        static StoredMessage Msg(string id, string relatedTo, string messageType, DateTime sent, string from, DateTime processed, string to) => new StoredMessage
        {
            MessageId = id,
            MessageType = messageType,
            ProcessedAt = processed,
            TimeSent = sent,
            SendingEndpoint = new Endpoint
            {
                Name = from
            },
            ReceivingEndpoint = new Endpoint
            {
                Name = to
            },
            Headers = new List<StoredMessageHeader>
                {
                    new StoredMessageHeader
                    {
                        Key = MessageHeaderKeys.RelatedTo,
                        Value = relatedTo
                    }
                }
        };
    }
}