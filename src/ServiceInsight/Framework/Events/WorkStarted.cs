namespace Particular.ServiceInsight.Desktop.Framework.Events
{
    public class WorkStarted
    {
        public WorkStarted()
            : this("Wait...")
        {
        }

        public WorkStarted(string message, params object[] args)
        {
            Message = string.Format(message, args);
        }

        public string Message { get; private set; }
    }
}