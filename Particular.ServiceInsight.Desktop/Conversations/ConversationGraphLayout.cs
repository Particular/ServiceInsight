using System;
using System.Collections.Generic;
using System.Windows;
using GraphSharp.Controls;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Conversations
{
    public class ConversationGraphLayout : GraphLayout<DiagramNode, MessageEdge, ConversationGraph>
    {
        public event EventHandler LayoutUpdateFinished;

        protected override void OnLayoutIterationFinished(IDictionary<DiagramNode, Point> vertexPositions, string message)
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