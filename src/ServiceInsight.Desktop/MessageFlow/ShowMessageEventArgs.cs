using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    public class SearchMessageEventArgs : EventArgs
    {
        public SearchMessageEventArgs(MessageNode message)
        {
            this.MessageNode = message;
        }

        public MessageNode MessageNode { get; private set; }
    }
}
