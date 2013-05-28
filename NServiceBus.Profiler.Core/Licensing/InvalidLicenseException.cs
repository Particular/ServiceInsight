using System;
using System.Runtime.Serialization;

namespace NServiceBus.Profiler.Core.Licensing
{
    public class InvalidLicenseException : ApplicationException
    {
        public InvalidLicenseException()
        {
        }

        public InvalidLicenseException(string message) : base(message)
        {
        }

        public InvalidLicenseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidLicenseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}