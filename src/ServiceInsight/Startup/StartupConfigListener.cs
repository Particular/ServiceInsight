namespace ServiceInsight.Startup
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using ServiceInsight.Framework.Events;

    public class StartupConfigListener
    {
        public static async Task Start(IEventAggregator eventAggregator, CommandLineArgParser parser, CancellationToken cancellation = default)
        {
            TcpListener listener = default;
            
            try
            {
                listener = new TcpListener(IPAddress.Loopback, CommandLineArgParser.ListenerPort);
                listener.Start();

                while (true)
                {
                    if (cancellation.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        await Handle(listener, eventAggregator, parser, cancellation);
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
            finally
            {
                listener?.Stop();
            }
        }
        
        static async Task Handle(TcpListener listener, IEventAggregator eventAggregator, CommandLineArgParser parser, CancellationToken cancellation)
        {
            using (cancellation.Register(listener.Stop))
            using (var client = await listener.AcceptTcpClientAsync())
            using (var reader = new StreamReader(client.GetStream()))
            {
                var args = await reader.ReadToEndAsync();

                if (args != null)
                {
                    parser.Parse(new[] {"", args});
                    
                    await eventAggregator.PublishOnUIThreadAsync(new ConfigurationUpdated(parser.ParsedOptions));
                }

                if (client.Connected)
                {
                    client.Close();
                }
            }
        }
    }
}