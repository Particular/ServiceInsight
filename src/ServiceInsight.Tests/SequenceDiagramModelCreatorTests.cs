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

            Assert.That(result.Count, Is.EqualTo(0));
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

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("A"));
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

            Assert.That(endpoints.Count, Is.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(endpoints[0].Name, Is.EqualTo("A"));
                Assert.That(endpoints[0].Host, Is.EqualTo(nameof(testHost)));
                Assert.That(endpoints[0].HostId, Is.EqualTo(testHost));
                Assert.That(endpoints[0].Hosts.Count, Is.EqualTo(1));
            });
            Assert.That(endpoints[0].Hosts[0].HostVersions.Count, Is.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(endpoints[0].Hosts[0].HostVersions[0], Is.EqualTo("1.0.0"));
                Assert.That(endpoints[0].Hosts[0].HostVersions[1], Is.EqualTo("2.0.0"));
                Assert.That(endpoints[0].Hosts[0].HostVersions[2], Is.EqualTo("3.0.0"));
            });
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

            Assert.That(result.Count, Is.EqualTo(4));
            Assert.Multiple(() =>
            {
                Assert.That(result[0].Name, Is.EqualTo("A"));
                Assert.That(result[1].Name, Is.EqualTo("B"));
                Assert.That(result[2].Name, Is.EqualTo("C"));
                Assert.That(result[3].Name, Is.EqualTo("D"));
            });
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

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result[0].Name, Is.EqualTo("A"));
                Assert.That(result[1].Name, Is.EqualTo("B"));
            });
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

            Assert.That(result.Count, Is.EqualTo(4));
            Assert.Multiple(() =>
            {
                Assert.That(result[0].Name, Is.EqualTo("A"));
                Assert.That(result[1].Name, Is.EqualTo("B"));
                Assert.That(result[2].Name, Is.EqualTo("C"));
                Assert.That(result[3].Name, Is.EqualTo("D"));
            });
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

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(result[0].Name, Is.EqualTo("A"));
                Assert.That(result[1].Name, Is.EqualTo("B"));
                Assert.That(result[2].Name, Is.EqualTo("C"));
            });
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

            Assert.Multiple(() =>
            {
                Assert.That(result[0].Name, Is.EqualTo("A"));
                Assert.That(result[0].Handlers.Count, Is.EqualTo(1));
                Assert.That(outArrows.Count, Is.EqualTo(3));
            });
            Assert.Multiple(() =>
            {
                Assert.That(outArrows[0].MessageId, Is.EqualTo("2"));
                Assert.That(outArrows[1].MessageId, Is.EqualTo("3"));
                Assert.That(outArrows[2].MessageId, Is.EqualTo("4"));
            });
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

            Assert.Multiple(() =>
            {
                Assert.That(result[0].Handlers.Count, Is.EqualTo(1));
                Assert.That(result[1].Handlers.Count, Is.EqualTo(2));
                Assert.That(result[2].Handlers.Count, Is.EqualTo(2));
                Assert.That(result[3].Handlers.Count, Is.EqualTo(1));
            });

            Assert.That(result[1].Handlers[0].Name, Is.EqualTo("Message1"));
            Assert.That(result[1].Handlers[1].Name, Is.EqualTo("Message5"));
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

            Assert.Multiple(() =>
            {
                Assert.That(result[0].Handlers.Count, Is.EqualTo(1));
                Assert.That(result[1].Handlers.Count, Is.EqualTo(4));
                Assert.That(result[0].Handlers[0].Out.Count(), Is.EqualTo(4));
            });
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

            Assert.That(result.Count, Is.EqualTo(5), "There should by 5 handlers");
            Assert.Multiple(() =>
            {
                Assert.That(result[0].ID, Is.EqualTo("1"), "Earliest handler should be for message 1 (even though we don't know what that was)");
                Assert.That(result[1].Name, Is.EqualTo("ProductPurchaseTakingTooLong"), "Second handler should be for ProductPurchaseTakingTooLong");
                Assert.That(result[2].ID, Is.EqualTo("2"), "Third handler is for message 2 (even though we don't know what that was)");
                Assert.That(result[3].Name, Is.EqualTo("GpOrderSagaTimeout"), "Fourth handler should be for the GpOrderSagaTimeout");
                Assert.That(result[4].Name, Is.EqualTo("CrmOrderSagaTimeout"), "Latest handler should be for CrmOrderSagaTimeout");
            });
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