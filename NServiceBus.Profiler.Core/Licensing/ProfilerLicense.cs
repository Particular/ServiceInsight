using System;

namespace NServiceBus.Profiler.Core.Licensing
{
    public class ProfilerLicense
    {
        public const string UnRegisteredUser = "Unregistered User";

        public DateTime ExpirationDate { get; set; }
        public string LicenseType { get; set; }
        public string RegisteredTo { get; set; }
        public string Version { get; set; }        
    }
}