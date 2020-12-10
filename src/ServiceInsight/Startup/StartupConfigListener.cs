using System.IO.Pipes;

namespace ServiceInsight.Startup
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Framework.Events;
    using Framework.Settings;
    
    public class StartupConfigListener
    {
        public static async Task Start(IEventAggregator eventAggregator, CommandLineArgParser parser, CancellationToken cancellation = default)
        {
            while (true)
            {
                if (cancellation.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    await Handle(eventAggregator, parser, cancellation);
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
        
        static async Task Handle(IEventAggregator eventAggregator, CommandLineArgParser parser, CancellationToken cancellation)
        {
            using (var pipe = new NamedPipeServerStream("ServiceInsight", PipeDirection.In, 1, PipeTransmissionMode.Byte))
            using (cancellation.Register(() => Disconnect(pipe)))
            {
                await pipe.WaitForConnectionAsync(cancellation);

                using (var reader = new StreamReader(pipe))
                {
                    var args = await reader.ReadToEndAsync();
                    if (args != null)
                    {
                        parser.Parse(new[] {"", args});

                        await eventAggregator.PublishOnUIThreadAsync(new ConfigurationUpdated(parser.ParsedOptions));
                    }
                }

                Disconnect(pipe);
            }
        }

        private static void Disconnect(NamedPipeServerStream pipe)
        {
            if (pipe?.IsConnected == true) 
            {
                pipe.Disconnect();
            }
        }
    }
}