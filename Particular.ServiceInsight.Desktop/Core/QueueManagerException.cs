using System;
using System.Runtime.Serialization;

namespace Particular.ServiceInsight.Desktop.Core
{
    [Serializable]
    public class QueueManagerException : Exception
    {
        public QueueManagerException()
        {
        }

        public QueueManagerException(string message) : base(message)
        {
        }

        public QueueManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected QueueManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}