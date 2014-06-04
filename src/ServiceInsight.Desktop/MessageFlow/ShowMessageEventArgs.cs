namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using System;

    public class SearchMessageEventArgs : EventArgs
    {
        public SearchMessageEventArgs(MessageNode message)
        {
            MessageNode = message;
        }

        public MessageNode MessageNode { get; private set; }
    }
}
