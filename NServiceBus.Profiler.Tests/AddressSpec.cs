using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Machine.Specifications;
using System.Linq;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Tests.Queues
{
    [Subject("address")]
    public class when_parsing_an_address_with_machine_name
    {
        protected static string FormatName;
        protected static Address Address;

        Establish context = () => FormatName = @"DIRECT=OS:mycomputer\private$\orderhandler";

        Because parsing_address_using_format_name = () => Address = Address.ParseFormatName(FormatName);

        It should_parse_the_address = () => Address.ShouldNotBeNull();
        It should_parse_the_machinename_correctly = () => Address.Machine.ShouldEqual("mycomputer");
        It should_parse_queue_name_correctly = () => Address.Queue.ShouldEqual("orderhandler");
        It should_parse_queue_type_correctly = () => Address.QueueType.ShouldEqual(QueueTypes.Private);
    }

    [Subject("address")]
    public class when_parsing_an_address_with_ip_address
    {
        protected static string FormatName;
        protected static Address Address;

        Establish context = () => FormatName = @"DIRECT=TCP:127.0.0.1\private$\orderhandler";

        Because parsing_address_using_format_name = () => Address = Address.ParseFormatName(FormatName);

        It should_parse_the_address = () => Address.ShouldNotBeNull();
        It should_parse_the_machinename_correctly = () => Address.Machine.ShouldEqual("127.0.0.1");
        It should_parse_queue_name_correctly = () => Address.Queue.ShouldEqual("orderhandler");
        It should_parse_queue_type_correctly = () => Address.QueueType.ShouldEqual(QueueTypes.Private);
    }

    [Subject("address")]
    public class when_parsing_an_address_without_direct_prefix
    {
        protected static string FormatName;
        protected static Address Address;

        Establish context = () => FormatName = @"127.0.0.1\private$\orderhandler";

        Because parsing_address_using_format_name = () => Address = Address.ParseFormatName(FormatName);

        It should_parse_the_address = () => Address.ShouldNotBeNull();
        It should_parse_the_machinename_correctly = () => Address.Machine.ShouldEqual("127.0.0.1");
        It should_parse_queue_name_correctly = () => Address.Queue.ShouldEqual("orderhandler");
        It should_parse_queue_type_correctly = () => Address.QueueType.ShouldEqual(QueueTypes.Private);
    }

    [Subject("address")]
    public class when_parsing_a_wrong_formatname
    {
        protected static string FormatName;
        protected static Address Address;
        protected static Exception Error;

        Establish context = () => FormatName = @"";

        Because parsing_address_using_format_name = () => Error = Catch.Exception(() => Address.ParseFormatName("orderhandler@127.0.0.1"));

        It should_parse_the_address = () => Address.ShouldBeNull();
        It should_throw_an_exception = () => Error.ShouldNotBeNull();
    }

    [Subject("address")]
    public class when_parsing_address_using_machine_name
    {
        protected static string QueueAddress;
        protected static Address Address;

        Establish context = () => QueueAddress = @"orderhandler@mycomputer";

        Because parsing_address = () => Address = Address.Parse(QueueAddress);

        It should_parse_the_address = () => Address.ShouldNotBeNull();
        It should_parse_the_machinename_correctly = () => Address.Machine.ShouldEqual("mycomputer");
        It should_parse_queue_name_correctly = () => Address.Queue.ShouldEqual("orderhandler");
        It should_parse_queue_type_correctly = () => Address.QueueType.ShouldEqual(QueueTypes.Private);
    }

    [Subject("address")]
    public class when_parsing_address_using_ip_address
    {
        protected static string QueueAddress;
        protected static Address Address;

        Establish context = () => QueueAddress = @"orderhandler@localhost";

        Because parsing_address = () => Address = Address.Parse(QueueAddress);

        It should_parse_the_address = () => Address.ShouldNotBeNull();
        It should_parse_the_machinename_correctly = () => Address.Machine.ShouldEqual(Environment.MachineName.ToLower());
        It should_parse_queue_name_correctly = () => Address.Queue.ShouldEqual("orderhandler");
        It should_parse_queue_type_correctly = () => Address.QueueType.ShouldEqual(QueueTypes.Private);
    }

    [Subject("address")]
    public class when_creating_address_with_no_machine_name_specified
    {
        protected static Address Address;

        Establish context = () => Address = new Address("orderhandler");

        It should_create_an_address = () => Address.ShouldNotBeNull();
        It should_point_to_the_right_queue = () => Address.Queue.ShouldEqual("orderhandler");
        It should_point_to_local_machine_name = () => Address.Machine.ShouldEqual(Environment.MachineName.ToLower());
    }

    [Subject("address")]
    public class when_converting_address_to_string
    {
        protected static string Address;

        Establish context = () =>
        {
            Address = new Address("remotemachine", "orderhandler").ToString();
        };

        It should_be_convertible_to_string = () => Address.ShouldNotBeNull();
        It should_contain_queue_name = () => Address.ShouldContain("orderhandler");
        It should_point_to_local_machine_name = () => Address.ShouldContain("remotemachine");
    }

    [Subject("address")]
    public class when_serializing_an_address
    {
        protected static Address before;
        protected static Address after;

        Establish context = () => before = new Address("remotemachine", "orderhandler");

        Because of = () =>
        {
            var serializer = new BinaryFormatter();
            var ms = new MemoryStream();

            serializer.Serialize(ms, before);
            ms.Position = 0;

            after = (Address) serializer.Deserialize(ms);
        };

        It should_be_able_to_deserialize_address = () => after.ShouldNotBeNull();
        It should_be_the_same_address = () => (before == after).ShouldBeTrue();
    }

    [Subject("address")]
    public class when_comparing_addresses
    {
        protected static Address FirstAddress;
        protected static Address SecondAddress = null;

        Because of = () => FirstAddress = new Address("remotemachine", "orderhandler");

        It should_return_true_when_comparing_to_same_ref = () => FirstAddress.Equals(FirstAddress).ShouldBeTrue();
        It should_return_true_when_comparing_to_same_ref_as_obj = () => FirstAddress.Equals((object)FirstAddress).ShouldBeTrue();
        It should_return_false_when_comparing_it_to_null_ref = () => FirstAddress.Equals(SecondAddress).ShouldBeFalse();
        It should_return_false_when_comparing_it_to_null_ref_as_obj = () => FirstAddress.Equals((object)SecondAddress).ShouldBeFalse();
        It should_not_be_the_same_as_a_null_ref = () => (FirstAddress != null).ShouldBeTrue();
        It should_not_be_anything_else = () => FirstAddress.Equals("A string").ShouldBeFalse();
    }

    [Subject("address")]
    public class when_checking_remote_addresses
    {
        It should_return_true_when_connecting_to_remote_ip_address = () => Address.IsRemote("192.9.9.2").ShouldBeTrue();
        It should_return_true_when_connecting_to_remote_machines = () => Address.IsRemote("myserver").ShouldBeTrue();
        It should_consider_network_ip_address_as_local = () => Address.IsRemote(GetLocalIp()).ShouldBeFalse();
        It should_consider_local_machine_name_as_local = () => Address.IsRemote(Environment.MachineName).ShouldBeFalse();

        private static string GetLocalIp()
        {
            var address = Dns.GetHostEntry(Dns.GetHostName())
                      .AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork)
                      .ToString();

            return address;
        }
    }

    [Subject("address")]
    public class when_checking_local_addresses
    {
        It should_consider_localhost_as_local = () => Address.IsLocal("localhost").ShouldBeTrue();
        It should_consider_loopback_ip_address_as_local = () => Address.IsLocal(IPAddress.Loopback.ToString()).ShouldBeTrue();
        It should_consider_dot_as_localhost = () => Address.IsLocal(".").ShouldBeTrue();
        It should_consider_local_machine_name_as_local = () => Address.IsLocal(Environment.MachineName);
    }

    [Subject("address")]
    public class when_validating_addresses
    {
        It should_consider_empty_strings_as_invalid = () => Address.IsValidAddress("").ShouldBeFalse();
        It should_consider_machine_name_valid = () => Address.IsValidAddress(Environment.MachineName).ShouldBeTrue();
        It should_consider_nonexisting_machines_invalid = () => Address.IsValidAddress("anynonexistingmachine").ShouldBeFalse();
    }
}