namespace Particular.ServiceInsight.Tests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using Desktop.Models;
    using Helpers;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class AddressTests
    {
        [Test]
        public void should_parse_the_address_from_direct_format_name()
        {
            var formatName = @"DIRECT=OS:mycomputer\private$\orderhandler";
            var address = Address.ParseFormatName(formatName);

            address.ShouldNotBe(null);
            address.Machine.ShouldBe("mycomputer");
            address.Queue.ShouldBe("orderhandler");
            address.QueueType.ShouldBe(QueueTypes.Private);
        }

        [Test]
        public void should_parse_an_address_with_ip_address()
        {
            var formatName = @"DIRECT=TCP:127.0.0.1\private$\orderhandler";

            var address = Address.ParseFormatName(formatName);

            address.ShouldNotBe(null);
            address.Machine.ShouldBe("127.0.0.1");
            address.Queue.ShouldBe("orderhandler");
            address.QueueType.ShouldBe(QueueTypes.Private);
        }

        [Test]
        public void should_parse_an_address_without_direct_prefix()
        {
            var formatName = @"127.0.0.1\private$\orderhandler";

            var address = Address.ParseFormatName(formatName);

            address.ShouldNotBe(null);
            address.Machine.ShouldBe("127.0.0.1");
            address.Queue.ShouldBe("orderhandler");
            address.QueueType.ShouldBe(QueueTypes.Private);
        }

        [Test]
        public void should_parse_address_using_machine_name()
        {
            var queue = @"orderhandler@mycomputer";
            var address = Address.Parse(queue);

            address.ShouldNotBe(null);
            address.Machine.ShouldBe("mycomputer");
            address.Queue.ShouldBe("orderhandler");
            address.QueueType.ShouldBe(QueueTypes.Private);
        }

        [Test]
        public void should_parse_address_using_ip_address()
        {
            var queue = @"orderhandler@localhost";

            var address = Address.Parse(queue);

            address.ShouldNotBe(null);
            address.Machine.ShouldBe(Environment.MachineName.ToLower());
            address.Queue.ShouldBe("orderhandler");
            address.QueueType.ShouldBe(QueueTypes.Private);
        }

        [Test]
        public void when_creating_address_with_no_machine_name_specified()
        {
            var address = new Address("orderhandler");

            address.Queue.ShouldBe("orderhandler");
            address.Machine.ShouldBe(Environment.MachineName.ToLower());
        }

        [Test]
        public void can_convert_address_to_string()
        {
            var address = new Address("remotemachine", "orderhandler").ToString();

            address.ShouldNotBe(null);
            address.ShouldContain("orderhandler");
            address.ShouldContain("remotemachine");
        }

        [Test]
        public void can_serialize_address_and_deserialize_back()
        {
            var before = new Address("remotemachine", "orderhandler");
            var after = BinaryObjectCloner.Clone(before);

            before.ShouldBe(after);
        }

        [Test]
        public void local_addresses_should_be_local()
        {
            Address.IsLocal("localhost").ShouldBe(true);
            Address.IsLocal(IPAddress.Loopback.ToString()).ShouldBe(true);
            Address.IsLocal(".").ShouldBe(true);
            Address.IsLocal(Environment.MachineName);
        }

        [Test]
        public void remote_addresses_should_be_remote()
        {
            Address.IsRemote("192.9.9.2").ShouldBe(true);
            Address.IsRemote("myserver").ShouldBe(true);
            Address.IsRemote(GetLocalIp()).ShouldBe(false);
            Address.IsRemote(Environment.MachineName).ShouldBe(false);
        }

        [Test]
        public void should_determine_if_an_address_is_invalid()
        {
            Address.IsValidAddress("").ShouldBe(false);
            Address.IsValidAddress(Environment.MachineName).ShouldBe(true);
            Address.IsValidAddress("anynonexistingmachine").ShouldBe(false);
        }

        [Test]
        public void parsing_a_wrong_formatname_will_throw()
        {
            Should.Throw<InvalidOperationException>(() => Address.ParseFormatName("orderhandler@127.0.0.1"));
        }

        [Test]
        public void parsing_an_empty_address_will_throw()
        {
            Should.Throw<InvalidOperationException>(() => Address.ParseFormatName(""));
        }

        [Test]
        public void parsing_a_null_address_will_throw()
        {
            Should.Throw<ArgumentNullException>(() => Address.ParseFormatName(null));
        }

        static string GetLocalIp()
        {
            var address = Dns.GetHostEntry(Dns.GetHostName())
                             .AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork)
                             .ToString();

            return address;
        }
    }
}