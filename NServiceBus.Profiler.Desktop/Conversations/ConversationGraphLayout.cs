using System;
using System.Collections.Generic;
using System.Windows;
using GraphSharp.Controls;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class ConversationGraphLayout : GraphLayout<StoredMessage, MessageEdge, ConversationGraph>
    {
        public event EventHandler LayoutUpdateFinished;

        protected override void OnLayoutIterationFinished(IDictionary<StoredMessage, Point> vertexPositions, string message)
        {
            base.OnLayoutIterationFinished(vertexPositions, message);
            var handler = LayoutUpdateFinished;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}