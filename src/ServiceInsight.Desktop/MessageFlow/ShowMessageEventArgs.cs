namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using System;

    public class SearchMessageEventArgs : EventArgs
    {
        public SearchMessageEventArgs(MessageNode message)
        {
            this.MessageNode = message;
        }

        public MessageNode MessageNode { get; private set; }
    }
}
