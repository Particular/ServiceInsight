namespace NServiceBus.Profiler.Core.Management
{
    public interface IManagementConnectionProvider
    {
        string Url { get; }
        void ConnectTo(string url);
    }

    public class ManagementConnectionProvider : IManagementConnectionProvider
    {
        public void ConnectTo(string url)
        {
            Url = url;
        }

        public string Url { get; private set; }
    }
}