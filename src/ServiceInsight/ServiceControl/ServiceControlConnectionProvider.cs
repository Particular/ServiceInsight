namespace ServiceInsight.ServiceControl
{
    public class ServiceControlConnectionProvider
    {
        public ServiceControlConnectionProvider()
        {
        }

        public void ConnectTo(string url, string username = null, string password = null)
        {
            Url = url;
            Username = username;
            Password = password;
            //eventAggregator.PublishOnUIThread(new ServiceControlConnectionChanged());
        }

        public string Url { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool UseWindowsAuthCustomUsernamePassword => !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password);
    }
}