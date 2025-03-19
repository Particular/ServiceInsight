namespace ServiceInsight.ServiceControl
{
    using System.Security;

    public class ServiceControlConnectionProvider
    {
        public ServiceControlConnectionProvider()
        {
        }

        public void ConnectTo(string url, string username = null, SecureString password = null)
        {
            Url = url;
            Username = username;
            Password = password ?? new SecureString();
            //eventAggregator.PublishOnUIThread(new ServiceControlConnectionChanged());
        }

        public string Url { get; private set; }
        public string Username { get; private set; }
        public SecureString Password { get; private set; }
        public bool UseWindowsAuthCustomUsernamePassword => !string.IsNullOrEmpty(Username) || (Password != null && Password.Length > 0);
    }
}