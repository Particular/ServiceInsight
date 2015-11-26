namespace Particular.ServiceInsight.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Desktop.Models;
    using global::ServiceInsight.SequenceDiagram;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    class SequenceDiagramModelCreatorTests
    {
        IMessageCommandContainer container;

        [SetUp]
        public void Setup()
        {
            container = Substitute.For<IMessageCommandContainer>();
        }

        ModelCreator GetModelCreator(List<ReceivedMessage> messages)
        {
            return new ModelCreator(messages, container);
        }

        [Test]
        public void NoMessages()
        {
            var messages = new List<ReceivedMessage>();
            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void SameSenderAndReceiver()
        {
            var messages = new List<ReceivedMessage>
            {
                new ReceivedMessage
                {
                    message_id = "1",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A",
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.Version,
                            value = "1"
                        },
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "10"
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
            var messages = new List<ReceivedMessage>
            {
                new ReceivedMessage
                {
                    message_id = "1",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A",
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.Version,
                            value = "1"
                        },
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "10"
                        }
                    }
                },
                new ReceivedMessage
                {
                    message_id = "2",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A",
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.Version,
                            value = "2"
                        },
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "10"
                        }
                    }
                },
                new ReceivedMessage
                {
                    message_id = "3",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A",
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.Version,
                            value = "3"
                        },
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "10"
                        }
                    }
                }
            };

            var creator = GetModelCreator(messages);
            var result = creator.Endpoints;

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual("A", result[1].Name);
            Assert.AreEqual("A", result[2].Name);
        }

        [Test]
        public void SequentialFlowWithDistinctEndpoints()
        {
            var messages = new List<ReceivedMessage>
            {
                new ReceivedMessage
                {
                    message_id = "3",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "D"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "2"
                        }
                    }
                },
                new ReceivedMessage
                {
                    message_id = "1",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>()
                },
                new ReceivedMessage
                {
                    message_id = "2",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
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
            var messages = new List<ReceivedMessage>
            {
                new ReceivedMessage
                {
                    message_id = "2",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
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
            var messages = new List<ReceivedMessage>
            {
                new ReceivedMessage
                {
                    message_id = "2",
                    processed_at = DateTime.UtcNow.AddMinutes(1),
                    sending_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    }
                },
                new ReceivedMessage
                {
                    message_id = "3",
                    processed_at = DateTime.UtcNow.AddMinutes(2),
                    sending_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "D"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    }
                },
                new ReceivedMessage
                {
                    message_id = "1",
                    processed_at = DateTime.UtcNow,
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>()
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
            var messages = new List<ReceivedMessage>
            {
                new ReceivedMessage
                {
                    message_id = "2",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    }
                },
                new ReceivedMessage
                {
                    message_id = "3",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    }
                },
                new ReceivedMessage
                {
                    message_id = "1",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>()
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

            var messages = new List<ReceivedMessage>
            {
                new ReceivedMessage
                {
                    message_id = "3",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "D"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    },
                    message_type = "Message3",
                    processed_at = currentDateTime.AddMinutes(2),
                    time_sent = currentDateTime.AddSeconds(3),
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
                },
                new ReceivedMessage
                {
                    message_id = "2",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    },
                    message_type = "Message2",
                    processed_at = currentDateTime.AddMinutes(1),
                    time_sent = currentDateTime.AddSeconds(2),
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
                },
                new ReceivedMessage
                {
                    message_id = "4",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    },
                    message_type = "Message4",
                    processed_at = currentDateTime.AddMinutes(1),
                    time_sent = currentDateTime.AddSeconds(4),
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
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

            var messages = new List<ReceivedMessage>
            {
                new ReceivedMessage
                {
                    message_id = "3",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "D"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "2"
                        }
                    },
                    message_type = "Message3",
                    processed_at = currentDateTime.AddMinutes(2),
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
                },
                new ReceivedMessage
                {
                    message_id = "1",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>(),
                    message_type = "Message1",
                    processed_at = currentDateTime,
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
                },
                new ReceivedMessage
                {
                    message_id = "2",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    },
                    message_type = "Message2",
                    processed_at = currentDateTime.AddMinutes(1),
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
                },
                new ReceivedMessage
                {
                    message_id = "4",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    },
                    message_type = "Message4",
                    processed_at = currentDateTime.AddMinutes(1),
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
                },
                new ReceivedMessage
                {
                    message_id = "5",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "C"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "4"
                        }
                    },
                    message_type = "Message5",
                    processed_at = currentDateTime.AddMinutes(2),
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
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

            var messages = new List<ReceivedMessage>
            {
                new ReceivedMessage
                {
                    message_id = "2",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    },
                    message_type = "Message2",
                    processed_at = currentDateTime,
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
                },
                new ReceivedMessage
                {
                    message_id = "3",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    },
                    message_type = "Message3",
                    processed_at = currentDateTime,
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
                },
                new ReceivedMessage
                {
                    message_id = "4",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    },
                    message_type = "Message4",
                    processed_at = currentDateTime,
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
                },
                new ReceivedMessage
                {
                    message_id = "5",
                    sending_endpoint = new EndpointAddress
                    {
                        name = "A"
                    },
                    receiving_endpoint = new EndpointAddress
                    {
                        name = "B"
                    },
                    headers = new List<Header>
                    {
                        new Header
                        {
                            key = MessageHeaderKeys.RelatedTo,
                            value = "1"
                        }
                    },
                    message_type = "Message5",
                    processed_at = currentDateTime,
                    message_intent = MessageIntent.Send,
                    status = MessageStatus.Successful,
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

            var messages = new List<ReceivedMessage>
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
        private static ReceivedMessage Msg(string id, string relatedTo, string messageType, DateTime sent, string from, DateTime processed, string to)
        {
            return new ReceivedMessage
            {
                message_id = id,
                message_type = messageType,
                processed_at = processed,
                time_sent = sent,
                sending_endpoint = new EndpointAddress
                {
                    name = from
                },
                receiving_endpoint = new EndpointAddress
                {
                    name = to
                },
                headers = new List<Header>
                {
                    new Header
                    {
                        key = MessageHeaderKeys.RelatedTo,
                        value = relatedTo
                    }
                }
            };
        }

    }
}