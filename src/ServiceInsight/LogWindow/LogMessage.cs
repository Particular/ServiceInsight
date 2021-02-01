namespace ServiceInsight.LogWindow
{
    using System.Windows;
    using System.Windows.Media;

    public class LogMessage
    {
        public LogMessage(string log, Color color, bool bold = false)
        {
            Log = log;
            Brush = new SolidColorBrush(color);
            Weight = bold ? FontWeights.Bold : FontWeights.Normal;
        }

        public string Log { get; }

        public Brush Brush { get; }

        public FontWeight Weight { get; }
    }
}