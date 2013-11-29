using Caliburn.PresentationFramework.Screens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Models
{
    [DebuggerDisplay("Type={ExceptionType},Message={Message}")]
    public class ExceptionDetails : IExceptionDetails
    {
        public ExceptionDetails() { }
        public ExceptionDetails(StoredMessage message)
        {
            ExceptionType = message.GetHeaderByKey(MessageHeaderKeys.ExceptionType);
            Message = message.GetHeaderByKey(MessageHeaderKeys.ExceptionMessage);
            Source = message.GetHeaderByKey(MessageHeaderKeys.ExceptionSource);
            StackTrace = message.GetHeaderByKey(MessageHeaderKeys.ExceptionStackTrace);
        }

        public string ExceptionType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
    }

    public interface IExceptionDetails 
    {
        string ExceptionType { get; set; }
        string Message { get; set; }
        string Source { get; set; }
        string StackTrace { get; set; }
    }
}
