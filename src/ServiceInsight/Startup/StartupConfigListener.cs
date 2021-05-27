namespace ServiceInsight.Startup
{
    using System.IO.Pipes;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;

    public class StartupConfigListener
    {
        public static async Task Start(IEventAggregator eventAggregator, CommandLineArgParser parser, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    await Handle(eventAggregator, parser, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    //when task is cancelled socket is disposed
                    break;
                }
            }
        }

        static async Task Handle(IEventAggregator eventAggregator, CommandLineArgParser parser, CancellationToken cancellationToken)
        {
            using (var pipe = new NamedPipeServerStream($"ServiceInsight-{Environment.UserName}", PipeDirection.In, 1, PipeTransmissionMode.Byte))
            using (cancellationToken.Register(() => Disconnect(pipe)))
            {
                await pipe.WaitForConnectionAsync(cancellationToken);

                using (var reader = new StreamReader(pipe))
                {
                    var args = await reader.ReadToEndAsync();
                    if (args != null)
                    {
                        parser.Parse(new[] { "", args });

                        await eventAggregator.PublishOnUIThreadAsync(new ConfigurationUpdated(parser.ParsedOptions));
                    }
                }

                Disconnect(pipe);
            }
        }

        static void Disconnect(NamedPipeServerStream pipe)
        {
            if (pipe?.IsConnected == true)
            {
                pipe.Disconnect();
            }
        }
    }
}