namespace Particular.ServiceInsight.Desktop.Models
{
    using System.Diagnostics;

    [DebuggerDisplay("Type={ExceptionType},Message={Message}")]
    public class ExceptionDetails 
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

}
